using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private bool isOn;

    [SerializeField] private bool isStartup = true;

    private Image buttonImage;

    public void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void Start()
    {
        buttonImage.sprite = isOn ? onSprite : offSprite;
    }

    public void SetOn()
    {
        isOn = true;
        buttonImage.sprite = onSprite;
    }

    public void SetOff()
    {
        if (isStartup)
        {
            isStartup = false;
            return;
        }
        isOn = false;
        buttonImage.sprite = offSprite;
    }

    public void Toggle()
    {
        isOn = !isOn;
        buttonImage.sprite = isOn ? onSprite : offSprite;
    }
}
