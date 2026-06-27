using UnityEngine;

public class mouse_look : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    float xRotation;
    float yRotation;

    public float mouseSens = 90f;
    /*private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSens;

            yRotation += mouseX;
            xRotation -= mouseY;
        }
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }*/
}
