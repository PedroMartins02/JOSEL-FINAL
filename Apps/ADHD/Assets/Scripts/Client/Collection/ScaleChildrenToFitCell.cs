using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ScaleCardsToFitCell : MonoBehaviour
{
    private GridLayoutGroup gridLayout;

    void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        ScaleCards();
    }


    private void ScaleCards()
    {
        Vector2 cellSize = gridLayout.cellSize;

        foreach (RectTransform card in transform)
        {
            if (card != null)
            {
                // Calculate scale factor based on cell size vs card's original size
                float scaleX = cellSize.x / card.rect.width;
                float scaleY = cellSize.y / card.rect.height;

                // Use the smaller scale to maintain the aspect ratio
                float uniformScale = Mathf.Min(scaleX, scaleY);

                // Apply uniform scale to the entire card
                card.localScale = new Vector3(uniformScale, uniformScale, 1);
                card.sizeDelta = cellSize; // Optional: Adjust if you need exact cell fitting
            }
        }
    }
}
