using UnityEngine;

public class AutoAssignEventCamera : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogWarning("AutoAssignEventCamera: Keine Kamera gefunden!");
            return;
        }

        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);

        foreach (Canvas canvas in allCanvases)
        {
            // Nur World-Space Canvas anpassen
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = cam;
            }
        }
    }
}