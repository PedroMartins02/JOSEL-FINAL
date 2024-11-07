using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Sprite UnitFrame;
    [SerializeField] private Sprite BattleTacticFrame;
    [SerializeField] private Sprite LegendFrame;

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

    [SerializeField] private Image CardArt;
    [SerializeField] private Image CardFrame;

    [SerializeField] private GameObject TagArea;

    void Start()
    {
        
    }

    public void SetCardData(CardSO card)
    {
        if (card == null)
            return;

        SetGeneralUI(card);

        if (card is UnitCardSO unitCard)
        {
            SetUnitCardUI(unitCard);
            return;
        }

        if (card is BattleTacticCardSO battleTacticCard)
        {
            SetBattleTacticCardUI(battleTacticCard);
            return;
        }

        if (card is LegendCardSO legendCard)
        {
            SetLegendCardUI(legendCard);
            return;
        }
    }

    private void SetGeneralUI(CardSO card)
    {
        CardName.text = card.Name;
        ShortText.text = card.ShortText;
        BlessingsText.text = card.Blessings.ToString();
        CivilizationName.text = card.Faction.ToString();
        //CardArt
    }

    private void SetUnitCardUI(UnitCardSO card)
    {
        CardFrame.sprite = UnitFrame;
        //AttackText.text = card.Attack;
        //HealthText.text = card.Text;
    }

    private void SetBattleTacticCardUI(BattleTacticCardSO card)
    {
        CardFrame.sprite = BattleTacticFrame;
        AttackIcon.SetActive(false);
        HealthIcon.SetActive(false);
        //Effects
    }

    private void SetLegendCardUI(LegendCardSO card)
    {
        CardFrame.sprite = LegendFrame;
        //AttackText.text = card.Attack;
        //HealthText.text = card.Text;
        //Effects
    }
}
