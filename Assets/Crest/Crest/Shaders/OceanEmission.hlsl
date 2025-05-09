// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#ifndef CREST_OCEAN_EMISSION_INCLUDED
#define CREST_OCEAN_EMISSION_INCLUDED

half3 ScatterColour
(
	in const half i_surfaceOceanDepth,
	in const float i_shadow,
	in const half sss,
	in const half3 i_view,
	in const half3 i_ambientLighting,
	in const half3 i_lightDir,
	in const half3 i_lightCol,
	in const half3 i_additionalLightCol,
	in const bool i_underwater
)
{
	// base colour
	float v = abs(i_view.y);
	// Previously caused rendering artifacts. See issue #1040.
	half3 col = lerp(_DiffuseGrazing, _Diffuse, v);

#if _SHADOWS_ON
	col = lerp(_DiffuseShadow, col, i_shadow);
#endif

#if _SUBSURFACESCATTERING_ON
	{
#if _SUBSURFACESHALLOWCOLOUR_ON
		float shallowness = pow(1. - saturate(i_surfaceOceanDepth / _SubSurfaceDepthMax), _SubSurfaceDepthPower);
		half3 shallowCol = _SubSurfaceShallowCol;
#if _SHADOWS_ON
		shallowCol = lerp(_SubSurfaceShallowColShadow, shallowCol, i_shadow);
#endif
		col = lerp(col, shallowCol, shallowness);
#endif

		col *= i_ambientLighting + i_lightCol;

		// Approximate subsurface scattering - add light when surface faces viewer. Use geometry normal - don't need high freqs.
		half towardsSun = pow(max(0., dot(i_lightDir, -i_view)), _SubSurfaceSunFallOff);
		// URP version was: col += (_SubSurfaceBase + _SubSurfaceSun * towardsSun) * _SubSurfaceColour.rgb * i_lightCol * shadow;
		half3 subsurface = (_SubSurfaceBase + _SubSurfaceSun * towardsSun) * _SubSurfaceColour.rgb * i_lightCol * i_shadow;
		if (!i_underwater)
		{
			subsurface *= (1.0 - v * v) * sss;
#if _ADDITIONAL_LIGHTS
			// Already includes attenuation from distance and shadows. Exclude from underwater.
			subsurface += _SubSurfaceColour.rgb * i_additionalLightCol;
#endif
		}
		col += subsurface;
	}
#endif // _SUBSURFACESCATTERING_ON

	return col;
}


#if _CAUSTICS_ON
void ApplyCaustics
(
	in const WaveHarmonic::Crest::TiledTexture i_causticsTexture,
	in const WaveHarmonic::Crest::TiledTexture i_distortionTexture,
	in const int2 i_positionSS,
	in const float3 i_scenePos,
	in const half3 i_lightDir,
	in const float i_sceneZ,
	in const bool i_underwater,
	inout half3 io_sceneColour,
	in const int i_sliceIndex,
	in const CascadeParams cascadeData
)
{
	// could sample from the screen space shadow texture to attenuate this..
	// underwater caustics - dedicated to P
	const float3 scenePosUV = WorldToUV(i_scenePos.xz, cascadeData, i_sliceIndex);

	float3 disp = 0.0;
	// this gives height at displaced position, not exactly at query position.. but it helps. i cant pass this from vert shader
	// because i dont know it at scene pos.
	SampleDisplacements(_LD_TexArray_AnimatedWaves, scenePosUV, 1.0, disp);
	half seaLevelOffset = _LD_TexArray_SeaFloorDepth.SampleLevel(LODData_linear_clamp_sampler, scenePosUV, 0.0).y;
	half waterHeight = _OceanCenterPosWorld.y + disp.y + seaLevelOffset;
	half sceneDepth = waterHeight - i_scenePos.y;
	// Compute mip index manually, with bias based on sea floor depth. We compute it manually because if it is computed automatically it produces ugly patches
	// where samples are stretched/dilated. The bias is to give a focusing effect to caustics - they are sharpest at a particular depth. This doesn't work amazingly
	// well and could be replaced.
	float mipLod = log2(max(i_sceneZ, 1.0)) + abs(sceneDepth - _CausticsFocalDepth) / _CausticsDepthOfField;
	// project along light dir, but multiply by a fudge factor reduce the angle bit - compensates for fact that in real life
	// caustics come from many directions and don't exhibit such a strong directonality
	// Removing the fudge factor (4.0) will cause the caustics to move around more with the waves. But this will also
	// result in stretched/dilated caustics in certain areas. This is especially noticeable on angled surfaces.
	float2 lightProjection = i_lightDir.xz * sceneDepth / (4.0 * i_lightDir.y);

	float3 cuv1 = 0.0; float3 cuv2 = 0.0;
	{
		float2 surfacePosXZ = i_scenePos.xz;
		float surfacePosScale = 1.37;

#if CREST_FLOATING_ORIGIN
		// Apply tiled shifting origin offset. Always needed.
		surfacePosXZ -= i_causticsTexture.FloatingOriginOffset();
		// Scale was causing popping.
		surfacePosScale = 1.0;
#endif

		surfacePosXZ += lightProjection;

		cuv1 = float3
		(
			surfacePosXZ / i_causticsTexture._scale + float2(0.044 * _CrestTime + 17.16, -0.169 * _CrestTime),
			mipLod
		);
		cuv2 = float3
		(
			surfacePosScale * surfacePosXZ / i_causticsTexture._scale + float2(0.248 * _CrestTime, 0.117 * _CrestTime),
			mipLod
		);
	}

	// We'll use this distortion code for above water in single pass due to refraction bug.
#if !defined(UNITY_SINGLE_PASS_STEREO) && !defined(UNITY_STEREO_INSTANCING_ENABLED)
	if (i_underwater)
#endif
	{
		float2 surfacePosXZ = i_scenePos.xz;

#if CREST_FLOATING_ORIGIN
		// Apply tiled shifting origin offset. Always needed.
		surfacePosXZ -= i_distortionTexture.FloatingOriginOffset();
#endif

		surfacePosXZ += lightProjection;

		half2 causticN = _CausticsDistortionStrength * UnpackNormal(i_distortionTexture.Sample(surfacePosXZ / i_distortionTexture._scale)).xy;
		cuv1.xy += 1.30 * causticN;
		cuv2.xy += 1.77 * causticN;
	}

	half causticsStrength = _CausticsStrength;

#if _SHADOWS_ON
#if defined(UNIVERSAL_PIPELINE_CORE_INCLUDED)
	{
		// Apply shadow maps to caustics.
		{
			// We could skip GetMainLight but this is recommended approach which is likely more robust to API changes.
			float4 shadowCoord = TransformWorldToShadowCoord(i_scenePos);
			Light mainLight = GetMainLight(TransformWorldToShadowCoord(i_scenePos));
			causticsStrength *= mainLight.shadowAttenuation;
		}
	}
#endif // UNIVERSAL_PIPELINE_CORE_INCLUDED
#endif // _SHADOWS_ON

	io_sceneColour.xyz *= 1.0 + causticsStrength *
	(
		0.5 * i_causticsTexture.SampleLevel(cuv1.xy, cuv1.z).xyz +
		0.5 * i_causticsTexture.SampleLevel(cuv2.xy, cuv2.z).xyz -
		_CausticsTextureAverage
	);
}
#endif // _CAUSTICS_ON

#if defined(UNIVERSAL_PIPELINE_CORE_INCLUDED)
half3 OceanEmission
(
	in const half3 i_view,
	in const half3 i_n_pixel,
	in const float3 i_lightDir,
	in const real3 i_grabPosXYW,
	in const float i_pixelZ,
	const float i_rawPixelZ,
	in const half2 i_uvDepth,
	in const int2 i_positionSS,
	in const float i_sceneZ,
	const float i_rawDepth,
	in const half3 i_bubbleCol,
	in const bool i_underwater,
	in const half3 i_scatterCol,
#if CREST_WATER_VOLUME
	in const bool i_backface,
#endif
	in const CascadeParams cascadeData0,
	in const CascadeParams cascadeData1
)
{
	half3 col = i_scatterCol;

	// underwater bubbles reflect in light
	col += i_bubbleCol;

#if _TRANSPARENCY_ON

	// View ray intersects geometry surface either above or below ocean surface

	const half2 uvBackground = i_grabPosXYW.xy / i_grabPosXYW.z;
	half3 sceneColour;
	half3 alpha = 0.;
	float depthFogDistance;

	half2 refractOffset = _RefractionStrength * i_n_pixel.xz;
	if (!i_underwater)
	{
		refractOffset *= min(1.0, 0.5 * (i_sceneZ - i_pixelZ)) / i_sceneZ;
	}

	float2 uvDepthRefract = i_uvDepth + refractOffset;
	uvDepthRefract = FoveatedRemapLinearToNonUniform(uvDepthRefract);

	float rawDepth = CREST_SAMPLE_SCENE_DEPTH_X(uvDepthRefract);

	bool caustics = true;
#if CREST_WATER_VOLUME_HAS_BACKFACE
	// Using uvDepthRefract works sometimes but not others.
	bool backface = ApplyVolumeToOceanSurfaceRefractions(i_positionSS + (refractOffset * _ScreenSize.xy), i_rawDepth, i_underwater, rawDepth, caustics);
#endif

	// Depth fog & caustics - only if view ray starts from above water
	if (!i_underwater)
	{
		half2 uvBackgroundRefract;
		float sceneZ = i_sceneZ;

		// Compute depth fog alpha based on refracted position if it landed on an underwater surface, or on unrefracted depth otherwise
		if (rawDepth < i_rawPixelZ)
		{
			uvBackgroundRefract = uvBackground + refractOffset;
			uvBackgroundRefract = FoveatedRemapLinearToNonUniform(uvBackgroundRefract);

#if CREST_WATER_VOLUME_HAS_BACKFACE
			if (!backface)
#endif
			{
				rawDepth = CREST_MULTISAMPLE_SCENE_DEPTH(uvBackgroundRefract, rawDepth);
			}
			sceneZ = CrestLinearEyeDepth(rawDepth);
			depthFogDistance = sceneZ - i_pixelZ;
		}
		else
		{
			// We have refracted onto a surface in front of the water. Cancel the refraction offset.
			uvBackgroundRefract = uvBackground;
			uvBackgroundRefract = FoveatedRemapLinearToNonUniform(uvBackgroundRefract);

			rawDepth = i_rawDepth;
#if CREST_WATER_VOLUME_HAS_BACKFACE
			if (!i_backface)
#endif
			{
				rawDepth = CREST_MULTISAMPLE_SCENE_DEPTH(uvBackground, rawDepth);
			}
			sceneZ = CrestLinearEyeDepth(rawDepth);
			// It seems that when MSAA is enabled this can sometimes be negative
			depthFogDistance = max(sceneZ - i_pixelZ, 0.0);
		}

		sceneColour = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uvBackgroundRefract).rgb;
#if _CAUSTICS_ON
#if CREST_WATER_VOLUME_HAS_BACKFACE
		if (caustics)
#endif
		{
		// Refractions don't work correctly in single pass. Use same code from underwater instead for now.
#if defined(UNITY_SINGLE_PASS_STEREO) || defined(UNITY_STEREO_INSTANCING_ENABLED)
			float3 scenePos = _WorldSpaceCameraPos - i_view * sceneZ / dot(UNITY_MATRIX_I_V._13_23_33, i_view);
#else
			float3 scenePos = ComputeWorldSpacePosition(uvBackgroundRefract, rawDepth, UNITY_MATRIX_I_VP);
#endif
			ApplyCaustics(_CausticsTiledTexture, _CausticsDistortionTiledTexture, uvBackgroundRefract * _ScreenSize.xy, scenePos, i_lightDir, sceneZ, i_underwater, sceneColour, _LD_SliceIndex + 1, cascadeData1);
		}
#endif
		alpha = 1.0 - exp(-_DepthFogDensity.xyz * depthFogDistance);
	}
	else
	{
		const half2 uvBackgroundRefract = rawDepth < i_rawPixelZ ? uvBackground + refractOffset : uvBackground;
		sceneColour = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uvBackgroundRefract).rgb;
		depthFogDistance = i_pixelZ;
		// keep alpha at 0 as UnderwaterReflection shader handles the blend
		// appropriately when looking at water from below
	}

	// blend from water colour to the scene colour
	col = lerp(sceneColour, col, alpha);

#endif // _TRANSPARENCY_ON

	return col;
}
#endif // UNIVERSAL_PIPELINE_CORE_INCLUDED

#endif // CREST_OCEAN_EMISSION_INCLUDED
