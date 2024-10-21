using UnityEngine;

public class ImageScaler : MonoBehaviour
{
    public RectTransform imageTransform; // The RectTransform of the image
    public Vector2 targetResolution = new Vector2(1920, 1080); // Target resolution you want to fit to

    void Start()
    {
        AdjustImageSize();
    }

    void AdjustImageSize()
    {
        // Get current resolution
        Vector2 screenResolution = new Vector2(Screen.width, Screen.height);

        // Scale image to fit screen or target resolution
        float widthRatio = screenResolution.x / targetResolution.x;
        float heightRatio = screenResolution.y / targetResolution.y;
        float scaleRatio = Mathf.Min(widthRatio, heightRatio);

        imageTransform.localScale = new Vector3(scaleRatio, scaleRatio, 1);
    }
}
