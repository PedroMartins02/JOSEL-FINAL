using UnityEngine;
using UnityEngine.UI;

public class ScaleCardToFitCell : MonoBehaviour
{
    private RectTransform parentRect;
    private bool isScaled = false;
    [SerializeField] private GameObject CardToScale;

    void Start()
    {
        parentRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isScaled) 
            return; 

        parentRect = (RectTransform) GetComponent<Transform>();
        ScaleChildren();
    }

    void ScaleChildren()
    {
        if (parentRect == null)
            return;

        Vector2 parentSize = parentRect.rect.size;

        if (parentSize.x > 0 && parentSize.y > 0)
            isScaled = true;

        RectTransform child = (RectTransform)CardToScale.transform;
        Vector2 childOriginalSize = child.rect.size;

        float scaleY = parentSize.y / childOriginalSize.y;
        float scaleX = scaleY;

        child.localScale = new Vector3(scaleX, scaleY, 1f);
        child.anchoredPosition = Vector2.zero;
    }
}
