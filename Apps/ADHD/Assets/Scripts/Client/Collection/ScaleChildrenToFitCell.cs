using UnityEngine;
using UnityEngine.UI;

public class ScaleCardsToFitCell : MonoBehaviour
{
    private RectTransform parentRect;
    private bool isScaled = false;

    void Start()
    {
        parentRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isScaled)
        {
            return;
        }
        parentRect = GetComponent<RectTransform>();
        ScaleChildren();
    }

    void ScaleChildren()
    {
        isScaled= true;
        if (parentRect == null)
        {
            return;
        }

        Vector2 parentSize = parentRect.rect.size;

        foreach (RectTransform child in transform)
        {
            Vector2 childOriginalSize = child.rect.size;

            float scaleX = parentSize.x / childOriginalSize.x;
            float scaleY = parentSize.y / childOriginalSize.y;

            child.localScale = new Vector3(scaleX, scaleY, 1f);
            child.anchoredPosition = Vector2.zero;
        }
    }
}
