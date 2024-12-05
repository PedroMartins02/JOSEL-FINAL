using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUI;
   
    void Start()
    {
        tutorialUI.SetActive(false);
    }

    public void OpenTutorial()
    {
        tutorialUI.SetActive(true);
    }
}
