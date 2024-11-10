using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuantityUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI QuantityText;
    public void SetQuantity(int quantity)
    {
        QuantityText.text = quantity.ToString();
    }
}
