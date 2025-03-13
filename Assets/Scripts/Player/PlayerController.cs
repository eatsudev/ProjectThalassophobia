using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform t;

    public static bool inWater;
    public static bool isSwimming;
    public LayerMask waterMask;

    [Header("Player Rotation")]
    public float sensitivity = 1;

    private float rotX;
    private float rotY;

    public float rotMin;
    public float rotMax;

    [Header("Player Movement")]
    public float speed = 1f;
    float moveX;
    float moveY;
    float moveZ;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = this.transform;

        Cursor.lockState = CursorLockMode.Locked;
        inWater = false;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log("Collided with terrain");
        }
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

    void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Forward");

        if (!inWater)
        {
            Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
            t.Translate(moveDirection * Time.deltaTime * speed, Space.Self);
        }
        else
        {
            moveY = Input.GetAxis("Vertical");

            Vector3 swimDirection = new Vector3(moveX, moveY, moveZ);
            t.Translate(swimDirection * Time.deltaTime * speed, Space.Self);
        }

    }

    
}
