using UnityEngine;

public class BasicCharacterController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 100.0f;

    private float verticalRotation = 0;

    void Start()
    {

        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Mouse look
        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0, rotLeftRight, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Movement
        float forwardSpeed = (Input.GetAxis("Vertical")) * movementSpeed;
        float sideSpeed = (Input.GetAxis("Horizontal")) * movementSpeed;

        Vector3 speed = new Vector3(sideSpeed, 0, forwardSpeed);
        speed = transform.rotation * speed;

        transform.position += speed * Time.deltaTime;
    }

}
