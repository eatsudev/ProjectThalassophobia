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
        SwimmingOrFloating();
        Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & waterMask) != 0)
        {
            inWater = true;
            rb.useGravity = false; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & waterMask) != 0)
        {
            inWater = false;
            rb.useGravity = true;
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
        //moveY = Input.GetAxis("Vertical");
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

    void SwitchMovement()
    {
        inWater = !inWater;
        rb.useGravity = !rb.useGravity;
    }

    void SwimmingOrFloating()
    {
        if (inWater)
        {
            RaycastHit hit;
            if (Physics.Raycast(t.position, Vector3.down, out hit, Mathf.Infinity, waterMask))
            {
                isSwimming = hit.distance > 0.1f;
            }
            else
            {
                isSwimming = true;
            }
        }
        else
        {
            isSwimming = false;
        }

        Debug.Log("isSwimming = " + isSwimming);
    }
}
