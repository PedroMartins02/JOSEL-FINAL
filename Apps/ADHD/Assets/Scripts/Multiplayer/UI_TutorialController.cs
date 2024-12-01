using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TutorialController : MonoBehaviour
{
    [Header("Button Sprite")]
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    [Header("Tutorial One")]
    [SerializeField] private GameObject tutorialOne;
    [SerializeField] private Image tutorialOneBtnImage;

    [Header("Tutorial Two")]
    [SerializeField] private GameObject tutorialTwo;
    [SerializeField] private Image tutorialTwoBtnImage;

    [Header("Tutorial Three")]
    [SerializeField] private GameObject tutorialThree;
    [SerializeField] private Image tutorialThreeBtnImage;

    [Header("Tutorial Four")]
    [SerializeField] private GameObject tutorialFour;
    [SerializeField] private Image tutorialFourBtnImage;

    private void Start()
    {
        Hide();

        tutorialOne.SetActive(true);
        tutorialTwo.SetActive(false);
        tutorialThree.SetActive(false);
        tutorialFour.SetActive(false);

        tutorialOneBtnImage.sprite = spriteOn;
        tutorialTwoBtnImage.sprite = spriteOff;
        tutorialThreeBtnImage.sprite = spriteOff;
        tutorialFourBtnImage.sprite = spriteOff;
    }

    public void ChangeTutorialShown(int tutorialInt)
    {
        Show(); 

        switch (tutorialInt)
        {
            case 1:
                tutorialOne.SetActive(true);
                tutorialTwo.SetActive(false);
                tutorialThree.SetActive(false);
                tutorialFour.SetActive(false);

                tutorialOneBtnImage.sprite = spriteOn;
                tutorialTwoBtnImage.sprite = spriteOff;
                tutorialThreeBtnImage.sprite = spriteOff;
                tutorialFourBtnImage.sprite = spriteOff;
                break;
            case 2:
                tutorialOne.SetActive(false);
                tutorialTwo.SetActive(true);
                tutorialThree.SetActive(false);
                tutorialFour.SetActive(false);

                tutorialOneBtnImage.sprite = spriteOff;
                tutorialTwoBtnImage.sprite = spriteOn;
                tutorialThreeBtnImage.sprite = spriteOff;
                tutorialFourBtnImage.sprite = spriteOff;
                break;
            case 3:
                tutorialOne.SetActive(false);
                tutorialTwo.SetActive(false);
                tutorialThree.SetActive(true);
                tutorialFour.SetActive(false);

                tutorialOneBtnImage.sprite = spriteOff;
                tutorialTwoBtnImage.sprite = spriteOff;
                tutorialThreeBtnImage.sprite = spriteOn;
                tutorialFourBtnImage.sprite = spriteOff;
                break;
            case 4:
                tutorialOne.SetActive(false);
                tutorialTwo.SetActive(false);
                tutorialThree.SetActive(false);
                tutorialFour.SetActive(true);

                tutorialOneBtnImage.sprite = spriteOff;
                tutorialTwoBtnImage.sprite = spriteOff;
                tutorialThreeBtnImage.sprite = spriteOff;
                tutorialFourBtnImage.sprite = spriteOn;
                break;
            default:
                break;
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}
