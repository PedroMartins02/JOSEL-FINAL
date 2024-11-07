using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    // Reference to the panel to destroy
    public GameObject panelToDestroy;

    public void DestroyPanel()
    {
        if (panelToDestroy != null)
        {
            Destroy(panelToDestroy);
        }
        else
        {
            Debug.LogWarning("Panel reference not set in the inspector!");
        }
    }
}
