using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Battle Tactic")]
    public class BattleTacticCardSO : CardSO
    {
        [Space]
        [Header("Effects")]
        public List<Effect> Effects;
    }

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
}