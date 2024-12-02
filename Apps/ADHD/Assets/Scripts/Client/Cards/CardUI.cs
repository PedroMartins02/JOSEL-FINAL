using Game.Data;
using Game.Logic;
using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("Frame Sprites")]
    [SerializeField] private Sprite UnitFrame;
    [SerializeField] private Sprite BattleTacticFrame;
    [SerializeField] private Sprite LegendFrame;

    [Header("Element Sprites")]
    [SerializeField] private Sprite FireSprite;
    [SerializeField] private Sprite AirSprite;
    [SerializeField] private Sprite EarthSprite;
    [SerializeField] private Sprite WaterSprite;
    [SerializeField] private Sprite LightSprite;
    [SerializeField] private Sprite DarkSprite;

    [Header("References")]
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
    [SerializeField] private TextMeshProUGUI LongText;

    [SerializeField] private Image CardArt;
    [SerializeField] private Image CardFrame;

    void Start()
    {

    }

    public void SetCardData(CardSO card)
    {
        if (card == null)
            return;

        SetGeneralUI(card);

        if (card is LegendCardSO legendCard)
        {
            SetLegendCardUI(legendCard);
            return;
        }

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
    }

    public void SetCardData(CardDataSnapshot cardData)
    {
        SetGeneralUI(cardData);

        UpdateCardData(cardData);
    }



    public void UpdateCardData(CardDataSnapshot cardData)
    {

        switch (cardData.CardType)
        {
            case CardType.Unit:
                SetUnitCardUI(cardData);
                break;
            case CardType.Legend:
                SetLegendCardUI(cardData);
                break;
            case CardType.BattleTactic:
                SetBattleTacticCardUI(cardData);
                break;
        }
    }

    private void SetGeneralUI(CardDataSnapshot cardData)
    {
        CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(cardData.Id);

        if (cardSO == null) return;

        CardName.text = cardSO.Name;
        ShortText.text = cardSO.ShortText;
        LongText.text = cardSO.Description;
        BlessingsText.text = cardSO.Blessings.ToString();
        CivilizationName.text = cardSO.Faction.ToString();
        CardArt.sprite = cardSO.Art;
    }

    private void SetGeneralUI(CardSO card)
    {
        CardName.text = card.Name;
        ShortText.text = card.ShortText;
        LongText.text = card.Description;
        BlessingsText.text = card.Blessings.ToString();
        CivilizationName.text = card.Faction.ToString();
        CardArt.sprite = card.Art;
        if (card.Element == Elements.Fire)
            ElementIcon.sprite = FireSprite;
        else if (card.Element == Elements.Air)
            ElementIcon.sprite = AirSprite;
        else if (card.Element == Elements.Earth)
            ElementIcon.sprite = EarthSprite;
        else if (card.Element == Elements.Water)
            ElementIcon.sprite = WaterSprite;
        else if (card.Element == Elements.Light)
            ElementIcon.sprite = LightSprite;
        else if (card.Element == Elements.Dark)
            ElementIcon.sprite = DarkSprite;
        else 
            ElementIcon.enabled = false;
    }


    private void SetUnitCardUI(CardDataSnapshot cardData)
    {
        CardFrame.sprite = UnitFrame;
        AttackText.text = cardData.CurrentAttack.ToString();
        HealthText.text = cardData.CurrentHealth.ToString();
    }

    private void SetBattleTacticCardUI(CardDataSnapshot cardData)
    {
        CardFrame.sprite = BattleTacticFrame;
        AttackIcon.SetActive(false);
        HealthIcon.SetActive(false);
        //Effects
    }

    private void SetLegendCardUI(CardDataSnapshot cardData)
    {
        CardFrame.sprite = LegendFrame;
        AttackText.text = cardData.CurrentAttack.ToString();
        HealthText.text = cardData.CurrentHealth.ToString();
        //Effects
    }

    private void SetUnitCardUI(UnitCardSO card)
    {
        CardFrame.sprite = UnitFrame;
        AttackText.text = card.Attack.ToString();
        HealthText.text = card.Health.ToString();
    }

    private void SetBattleTacticCardUI(BattleTacticCardSO card)
    {
        CardFrame.sprite = BattleTacticFrame;
        AttackIcon.SetActive(false);
        HealthIcon.SetActive(false);
        //Effects
    }

    private void SetBattleTacticCardUI(BattleTacticCard card)
    {
        CardFrame.sprite = BattleTacticFrame;
        AttackIcon.SetActive(false);
    }
    private void SetLegendCardUI(LegendCardSO card)
    {
        CardFrame.sprite = LegendFrame;
        AttackText.text = card.Attack.ToString();
        HealthText.text = card.Health.ToString();
        //Effects
    }

    private void SetLegendCardUI(LegendCard card)
    {
        CardFrame.sprite = LegendFrame;
        AttackText.text = card.CurrentAttack.ToString();
        HealthText.text = card.CurrentHealth.ToString();
        //Effects
    }
}
