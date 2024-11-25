using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;
using Game.Data;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Legend Card", menuName = "Cards/Legend Card")]
    public class LegendCardSO : UnitCardSO
    {
        public override CardData CardData
        {
            get
            {
                return new LegendCardData(
                    Id, Name, ShortText, Description,
                    Blessings, Health, Attack,
                    Effects,
                    Faction, Element
                    );
            }
        }

        [Space]
        [Header("Effects")]
        public List<Effect> Effects;


    }
}
#region Custom Editor
#if UNITY_EDITOR
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
#endif
#endregion