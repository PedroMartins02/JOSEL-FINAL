using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public RectTransform skyImage;  // Drag the sky object into this field
    public float speed = 0.5f;  // Speed of the cloud movement
    public float resetPositionX = -10f;  // Position where the cloud image resets
    public float startPositionX = 10f;   // Starting position of the cloud image

    void Update()
    {
        // Move the sky image horizontally based on the speed
        skyImage.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        // Check if the sky image has moved past the reset position
        if (skyImage.anchoredPosition.x <= resetPositionX)
        {
            // Reset the position to the start
            skyImage.anchoredPosition = new Vector2(startPositionX, skyImage.anchoredPosition.y);
        }
    }
}
