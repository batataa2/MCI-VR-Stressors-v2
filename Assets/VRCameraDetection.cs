using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRAutoSwitcher : MonoBehaviour
{
    [Header("References")]
    public GameObject xrRig;                 // XR Origin (XR Rig)
    public Camera desktopCamera;             // Deine Desktop Camera
    public AudioListener desktopAudioListener;
    public AudioListener xrAudioListener;

    void Start()
    {
        CheckForVR();
    }

    void CheckForVR()
    {
        bool vrActive = false;

        List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(displays);

        foreach (var d in displays)
        {
            if (d.running)
            {
                vrActive = true;
                break;
            }
        }

        if (vrActive)
            ActivateVR();
        else
            ActivateDesktop();
    }

    void ActivateVR()
    {
        xrRig.SetActive(true);

        if (desktopCamera != null)
            desktopCamera.gameObject.SetActive(false);

        // Audio wechseln
        if (desktopAudioListener != null)
            desktopAudioListener.enabled = false;

        if (xrAudioListener != null)
            xrAudioListener.enabled = true;

        Debug.Log("VR HEADSET DETECTED → XR Rig aktiviert");
    }

    void ActivateDesktop()
    {
        xrRig.SetActive(false);

        if (desktopCamera != null)
            desktopCamera.gameObject.SetActive(true);

        // Audio wechseln
        if (desktopAudioListener != null)
            desktopAudioListener.enabled = true;

        if (xrAudioListener != null)
            xrAudioListener.enabled = false;

        Debug.Log("Kein VR Headset → Desktop Camera aktiviert");
    }
}