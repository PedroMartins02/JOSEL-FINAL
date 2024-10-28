using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image ElementIcon;

    [SerializeField] private GameObject BlessingsIcon;
    [SerializeField] private GameObject AttackIcon;
    [SerializeField] private GameObject HealthIcon;

    [SerializeField] private TextMeshProUGUI BlessingsText;
    [SerializeField] private TextMeshProUGUI AttackText;
    [SerializeField] private TextMeshProUGUI HealthText;

    [SerializeField] private TextMeshProUGUI CardName;
    [SerializeField] private TextMeshProUGUI CivilizationName;
    [SerializeField] private TextMeshProUGUI ShortText;

    [SerializeField] private GameObject TagArea;

    void Start()
    {
        
    }

    public void SetCardData(Card card)
    {

    }
}
