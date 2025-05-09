using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform t;

    public static bool inWater;
    public static bool isSwimming;
    public LayerMask waterMask;
    private float waterSurfaceY;
    public float playerHeight = 1.0f;
    public bool canLook = true;
    public float waterSurfaceHeight = 50f;
    public float maxAllowedHeightBelowSurface = 0.5f;

    [Header("Player Rotation")]
    public float sensitivity = 1;

    private float rotX;
    private float rotY;

    public float rotMin = -50;
    public float rotMax = 50;

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
        TerrainCollide();
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
        WaterMovement();
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void LookAround()
    {
        if (!canLook) return;

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


    private void TerrainCollide()
    {
        RaycastHit hit;
        float rayLength = playerHeight + 0.5f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                float groundY = hit.point.y + playerHeight;
                if (transform.position.y < groundY)
                {
                    Vector3 correctedPos = new Vector3(transform.position.x, groundY, transform.position.z);
                    transform.position = correctedPos;
                }
            }
        }
    }

    private void WaterMovement()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, waterMask))
        {
            waterSurfaceY = hit.point.y;
        }

        if (transform.position.y >= waterSurfaceY)
        {
            transform.position = new Vector3(transform.position.x, waterSurfaceY, transform.position.z);
        }

        Vector3 pos = transform.position;
        float maxY = waterSurfaceHeight - maxAllowedHeightBelowSurface;
        if (pos.y > maxY)
        {
            pos.y = maxY;
            transform.position = pos;
        }
    }
    
}
