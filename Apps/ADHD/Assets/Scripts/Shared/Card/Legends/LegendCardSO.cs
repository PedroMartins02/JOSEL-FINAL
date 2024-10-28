using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Legend Card", menuName = "Cards/Legend Card")]
public class LegendCardSO : CardSO
{
    public int Attack;
    public int Health;

    [Space]
    [Header("Effects")]
    public List<Effect> Effects;

    public UnitCardSO MapToUnitCardSO()
    {
        UnitCardSO unitCardSO = new()
        {
            Name = Name,
            Art = Art,
            Faction = Faction,
            Element = Element,
            ShortText = ShortText,
            Description = Description,
            Blessings = Blessings,
            Attack = Attack,
            Health = Health,
        };

        return unitCardSO;
    }

    [CustomEditor(typeof(LegendCardSO))]
    public class LegendSOEditor : CardSOEditor
    {
        private void OnEnable()
        {
            card = target as LegendCardSO;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
