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
    private float moveX;
    private float moveY;
    private float moveZ;

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

        if (!inWater)
        {
            t.Translate(new Quaternion(0, t.rotation.y, 0, t.rotation.w) * new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed, Space.World);
        }
        else
        {
            if(!isSwimming)
            {
                moveY = Mathf.Min(moveY, 0);
                Vector3 clampedDirection = t.TransformDirection(new Vector3(moveX, moveY, moveZ));
                clampedDirection = new Vector3(clampedDirection.x, Mathf.Min(clampedDirection.y, 0), clampedDirection.z);
                t.Translate(clampedDirection * Time.deltaTime * speed, Space.World);
            }
            else
            {
                t.Translate(new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed);
                t.Translate(new Vector3(0, moveY, 0) * Time.deltaTime * speed, Space.World);
            }
        }
        
    }

    private void SwitchMovement()
    {
        inWater = !inWater;
        rb.useGravity = !rb.useGravity;
    }

    private void SwimmingOrFloating()
    {
        bool swimCheck = false;

        if (inWater)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(t.position.x, t.position.y + 0.5f, t.position.z), Vector3.down, out hit, Mathf.Infinity, waterMask))
            {
                if(hit.distance < 0.1f)
                {
                    swimCheck = true;
                }
            }
            else
            {
                swimCheck = false;
            }
        }
        
        isSwimming = swimCheck;
        Debug.Log("isSwimming = " + isSwimming);
    }
}
