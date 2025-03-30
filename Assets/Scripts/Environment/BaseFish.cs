using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFish : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float turnSpeed = 2f;
    public float changeDirectionTime = 3f; 

    public Vector3 swimDirection;
    private float timer;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PickNewDirection();
    }

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            PickNewDirection();
        }

        MoveFish();
    }

    void PickNewDirection()
    {
        swimDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        timer = changeDirectionTime;
    }

    public void MoveFish()
    {
        if (swimDirection.magnitude > 0.1f)
        {
            if (rb != null)
            {
                rb.velocity = swimDirection * swimSpeed;
            }
            else
            {
                transform.position += swimDirection * swimSpeed * Time.deltaTime;
            }

            Quaternion targetRotation = Quaternion.LookRotation(swimDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
}
