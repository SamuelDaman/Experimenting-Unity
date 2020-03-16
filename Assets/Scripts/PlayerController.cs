using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private Camera cam;

    private bool isLocked = true;
    [SerializeField] private bool isGrounded;

    float mouseX = 0;
    float mouseY = 0;

    float verticalInput;
    float horizontalInput;

    float speed = 5;
    float verticalSpeed = 0;
    float airTime = 0;

    Vector3 boost;

    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
        cam = gameObject.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseLockState();
        PlayerMove();
        GroundCheck();
    }

    void GroundCheck()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(0, -1, 0), 1.1f))
        {
            isGrounded = true;
            airTime = 0;
            verticalSpeed = 0;
        }
        else
        {
            isGrounded = false;
            verticalSpeed -= Mathf.Pow(airTime, 1.01f);
            verticalSpeed = Mathf.Clamp(verticalSpeed, -50, 50);
            airTime += Time.deltaTime;
        }
    }

    void MoveInput()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void PlayerMove()
    {
        MoveInput();
        body.velocity = (transform.TransformDirection(horizontalInput, 0, verticalInput).normalized * speed) +
            new Vector3(0, verticalSpeed, 0) +
            boost;
    }

    void MouseLockState()
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            MouseLook();
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                isLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            if (Input.GetKeyDown(KeyCode.P))
            {
                isLocked = true;
            }
        }
    }

    void MouseLook()
    {
        mouseX += Input.GetAxis("Mouse X") * 5;
        mouseY += Input.GetAxis("Mouse Y") * 3;

        mouseY = Mathf.Clamp(mouseY, -90, 90);

        transform.eulerAngles = new Vector3(0, mouseX, 0);
        cam.transform.localEulerAngles = new Vector3(-mouseY, 0, 0);
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] points = new ContactPoint[collision.contactCount];
        collision.GetContacts(points);
        if (Input.GetButtonDown("Jump"))
        {
            Vector3 contactAverage = Vector3.zero;
            for (int i = 0; i < points.Length; i++)
            {
                contactAverage += points[i].point;
            }
            boost = -(contactAverage - transform.position).normalized;
        }
    }
}
