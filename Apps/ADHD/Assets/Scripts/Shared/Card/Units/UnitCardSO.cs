using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Unit Card", menuName = "Cards/Unit Card")]
    public class UnitCardSO : CardSO
    {
        public int Attack;
        public int Health;
    }

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
}