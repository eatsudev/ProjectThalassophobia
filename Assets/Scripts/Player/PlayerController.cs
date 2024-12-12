using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform t;

    public static bool inWater;
    public static bool isSwimming;

    [Header("Player Rotation")]
    public float sensitivity = 1;

    private float rotX;
    private float rotY;

    public float rotMin;
    public float rotMax;

    [Header("Player Movement")]
    public float speed = 1f;
    private float moveX;
    private float moveY;
    private float moveZ;

    void Start()
    {
        t = this.transform;

        Cursor.lockState = CursorLockMode.Locked;
        inWater = false;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        SwitchMovement();
    }

    private void OnTriggerExit(Collider other)
    {
        SwitchMovement();
    }

    void Update()
    {
        LookAround();

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void LookAround()
    {
        rotX += Input.GetAxis("Mouse X") * sensitivity;
        rotY += Input.GetAxis("Mouse Y") * sensitivity;

        rotY = Mathf.Clamp(rotY, rotMin, rotMax);

        t.localRotation = Quaternion.Euler(-rotY, rotX, 0);
    }

    private void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Forward");

        t.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed);
        t.Translate(new Vector3(0, moveY, 0) * Time.deltaTime * speed, Space.World);
    }

    private void SwitchMovement()
    {
        inWater = !inWater;
    }
}
