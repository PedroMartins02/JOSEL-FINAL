using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Image mythImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI blessingsText;

    public void SetMythImage(Sprite image)
    {
        mythImage.sprite = image;
    }

    public void SetHealth(int health)
    {
        healthText.text = health.ToString();
    }

    public void SetBlessings(int blessings)
    {
        blessingsText.text = blessings.ToString();
    }
}
