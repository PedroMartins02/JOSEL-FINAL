using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythUI : MonoBehaviour
{
    [SerializeField] private Image MythImage;
    public void SetMythData(MythCardSO mythSO)
    {
        MythImage.sprite = mythSO.Art;
    }
}
