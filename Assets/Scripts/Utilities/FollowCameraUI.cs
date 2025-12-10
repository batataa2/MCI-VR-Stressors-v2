using UnityEngine;

public class FollowCameraUI : MonoBehaviour
{
    public Transform cameraTarget; 
    public float distance = 1.5f;   // Abstand vor dem Kopf
    public float height = -0.5f;    // leicht nach unten versetzt

    void LateUpdate()
    {
        if (cameraTarget == null) return;

        // Position direkt vor dem Kopf
        transform.position = cameraTarget.position 
                           + cameraTarget.forward * distance
                           + cameraTarget.up * height;

        // UI soll Kamera anschauen
        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTarget.position
        );
    }
}
