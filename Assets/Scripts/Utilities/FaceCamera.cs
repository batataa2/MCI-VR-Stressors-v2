using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;   // VR MainCamera
    }

    void LateUpdate()
    {
        // Canvas immer direkt zur Kamera drehen
        transform.LookAt(transform.position + cam.forward);
    }
}
