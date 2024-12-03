using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurrenderUI : MonoBehaviour
{
    [SerializeField] GameObject SurrenderScreen;

    private void Start()
    {
        ClosePopUp();
    }

    public void OpenPopUp()
    {
        SurrenderScreen.SetActive(true);
    }

    public void ClosePopUp()
    {
        SurrenderScreen.SetActive(false);
    }
}
