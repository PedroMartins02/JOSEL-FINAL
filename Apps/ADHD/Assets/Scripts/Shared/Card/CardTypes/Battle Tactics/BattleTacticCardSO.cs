using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Battle Tactic")]
    public class BattleTacticCardSO : CardSO
    {
        public override CardData CardData
        {
            get
            {
                return new BattleTacticCardData(
                    Id, Name, ShortText, Description,
                    Blessings,
                    Effects,
                    Faction, Element
                    );
            }
        }

        [Space]
        [Header("Effects")]
        public List<Effect> Effects;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BattleTacticCardSO))]
    public class BattleTacticCardSOEditor : CardSOEditor
    {
        private void OnEnable()
        {
            card = target as BattleTacticCardSO;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}