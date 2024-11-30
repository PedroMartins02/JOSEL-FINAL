using Oculus.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class CameraSwitchManager : MonoBehaviour
{
    [Header("PC related")]
    [SerializeField] private Camera pcCamera;
    [SerializeField] private Canvas canvas;

    [Header("Quest related")]
    [SerializeField] private GameObject vrRig;
    [SerializeField] private GameObject ISDK_PokeInteraction;
    [SerializeField] private GameObject RootProjectContent;
    [SerializeField] private PointableCanvasModule canvasModule;


    private void Awake()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        //UpdateCameraAndCanvas();
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
            DoSwitchToVR(true);
        }
        else
        {
            DoSwitchToVR(false);
        }
    }

    private void DoSwitchToVR(bool isVR)
    {
        // Enable the vr rig c:
        if (vrRig != null) 
            vrRig.gameObject.SetActive(isVR);
        if (ISDK_PokeInteraction != null)
            ISDK_PokeInteraction.gameObject.SetActive(isVR);
        if (canvasModule != null)
            canvasModule.enabled = isVR;

        if (RootProjectContent != null)
        {
            foreach (Component component in RootProjectContent.GetComponents<Component>())
            {
                // Check if the component is not Transform and not CameraSwitchManager
                if (!(component is Transform) && !(component is CameraSwitchManager))
                {
                    if (component is Behaviour behaviour)
                    {
                        behaviour.enabled = isVR;
                    }
                    else if (component is Renderer renderer)
                    {
                        renderer.enabled = isVR;
                    }
                    else if (component is Collider collider)
                    {
                        collider.enabled = isVR;
                    }
                    else
                    {
                        Debug.LogWarning($"Unable to check component: {component.GetType().Name}");
                    }
                }
            }
        }

        if (pcCamera != null) 
            pcCamera.enabled = !isVR;

        if (canvas != null)
        {
            if(isVR)
                canvas.renderMode = RenderMode.WorldSpace;
                //canvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
