using UnityEngine;

public class DesktopPlayerSimpleMouse : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float mouseSensitivity = 2f;
    public float arrowKeyTurnSpeed = 120f;
    public float fixedY = 2f;

    private float pitch = 0f;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        HandleMouseLook();
        HandleArrowKeyLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Horizontal drehen
        transform.Rotate(0f, mx, 0f);

        // Vertikal neigen
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // NEW: Arrow-key camera control (UP/DOWN = pitch, LEFT/RIGHT = yaw)
    void HandleArrowKeyLook()
    {
        // Left / Right rotation
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(0f, -arrowKeyTurnSpeed * Time.deltaTime, 0f);

        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(0f, arrowKeyTurnSpeed * Time.deltaTime, 0f);

        // Up / Down for pitch (camera tilt)
        if (Input.GetKey(KeyCode.UpArrow))
            pitch -= arrowKeyTurnSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.DownArrow))
            pitch += arrowKeyTurnSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * v + transform.right * h;
        dir.y = 0f;

        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }
}