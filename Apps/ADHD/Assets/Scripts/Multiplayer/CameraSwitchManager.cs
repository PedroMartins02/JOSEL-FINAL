using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class CameraSwitchManager : MonoBehaviour
{
    [SerializeField] private Camera pcCamera;
    [SerializeField] private GameObject vrRig;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        UpdateCameraAndCanvas();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCameraAndCanvas();
    }

    public void UpdateCameraAndCanvas()
    {
        /*
        // OBtain the VR rig, the canvas and the main camera
        pcCamera = Camera.main;
        
        GameObject vrRig = GameObject.Find("OVRCameraRigInteraction");
        if (vrRig != null)
        {
            this.vrRig = vrRig;
        }

        canvas = FindObjectOfType<Canvas>();
        */

        // Switch settings
        if (XRSettings.isDeviceActive)
        {
            SwitchToVR();
        }
        else
        {
            SwitchToPC();
        }
    }

    private void SwitchToVR()
    {
        // Enable the vr rig c:
        if (vrRig != null) vrRig.gameObject.SetActive(true);
        if (pcCamera != null) pcCamera.gameObject.SetActive(true);

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            //canvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }
    }

    private void SwitchToPC()
    {
        // Disable the vr rig :c
        if (vrRig != null) vrRig.gameObject.SetActive(false);
        if (pcCamera != null) pcCamera.gameObject.SetActive(true);

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
