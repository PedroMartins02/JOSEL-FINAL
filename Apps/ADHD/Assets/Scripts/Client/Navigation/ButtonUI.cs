using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private Image buttonImage;

    public void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void Start()
    {
        //SetOff();
    }

    public void SetOn()
    {
        buttonImage.sprite = onSprite;
    }

    public void SetOff()
    {
        buttonImage.sprite = offSprite;
    }
}
