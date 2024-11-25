using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit Card")]
    public class UnitCardSO : CardSO
    {
        public override CardData CardData
        {
            get
            {
                return new UnitCardData(
                    Id, Name, ShortText, Description, 
                    Blessings, Health, Attack,
                    Faction, Element
                    ); 
            }
        }

        public int Attack;
        public int Health;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UnitCardSO))]
    public class UnitCardSOEditor : CardSOEditor
    {
        private void OnEnable()
        {
            card = target as UnitCardSO;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}