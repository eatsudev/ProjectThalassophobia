using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolingFish : BaseFish
{
    public float neighborDistance = 5f;
    public float cohesionStrength = 0.5f;

    void Update()
    {
        Vector3 cohesionDirection = Vector3.zero;
        int neighborCount = 0;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborDistance);
        foreach (Collider neighbor in neighbors)
        {
            if (neighbor != this && neighbor.CompareTag("Fish"))
            {
                cohesionDirection += neighbor.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            cohesionDirection /= neighborCount;
            swimDirection = (cohesionDirection - transform.position).normalized;
        }

        MoveFish();
    }
}
