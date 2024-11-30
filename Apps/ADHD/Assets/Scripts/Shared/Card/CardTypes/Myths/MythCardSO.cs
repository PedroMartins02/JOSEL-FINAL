using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;
using Game.Data;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Myth Card", menuName = "Cards/Myth Card")]
public class MythCardSO : CardSO
{
    public override CardData CardData
    {
        get
        {
            return new CardData(
                Id, Name, ShortText, Description,
                Blessings,
                Faction, Element
                );
        }
    }

    [Space]
    [Header("Effects")]
    public List<Effect> Effects;


#if UNITY_EDITOR
    [CustomEditor(typeof(MythCardSO))]
    public class MythSOEditor : CardSOEditor
    {
        private void OnEnable()
        {
            card = target as MythCardSO;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}
