using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythUI : MonoBehaviour
{
    [SerializeField] private Image MythImage;

    private MythCardSO mythSO;

    public void SetMythData(MythCardSO mythSO)
    {
        this.mythSO = mythSO;
        MythImage.sprite = mythSO.Art;
    }
}
