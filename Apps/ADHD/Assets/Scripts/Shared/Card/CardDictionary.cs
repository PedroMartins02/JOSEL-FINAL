using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameModel
{
    [CreateAssetMenu(fileName = "New Card Dictionary", menuName = "Cards/Dictionary")]
    public class CardDictionarySO : ScriptableObject
    {
        [SerializeField] private CardDict _cardDict; 

        private Dictionary<string, CardSO> _cardDictionary;

        public Dictionary<string, CardSO> GetDictionary()
        {
            if (_cardDictionary == null)
                _cardDictionary = _cardDict.ToDictionary();

            return _cardDictionary;
        }

        public void UpdateDictionary(CardDict newCardDict)
        {
            if (_cardDictionary != null)
                _cardDictionary.Clear();

            _cardDict = null;
            _cardDict = newCardDict;

            _cardDictionary = _cardDict.ToDictionary();
        }
    }

    [Serializable]
    public class CardDict
    {
        [SerializeField]
        public List<CardDictItem> DictItems = new();

        public Dictionary<string, CardSO> ToDictionary()
        {
            Dictionary<string, CardSO> newDict = new Dictionary<string, CardSO>();

            foreach (var item in DictItems)
            {
                newDict.Add(item.Id, item.ItemCardSO);
            }

            return newDict;
        }
    }

    [Serializable]
    public struct CardDictItem
    {
        [SerializeField] public string Id;
        [SerializeField] public CardSO ItemCardSO;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CardDictionarySO))]
    public class CardDictionarySOEditor : Editor
    {
        private CardDictionarySO dictionary;

        private SerializedProperty m_cardDictionary;

        private void OnEnable()
        {
            dictionary = target as CardDictionarySO;

            m_cardDictionary = serializedObject.FindProperty("_dictionary");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(25);

            if (GUILayout.Button("Update Dictionary"))
            {
                UpdateDictionary();
            }
        }

        private void UpdateDictionary()
        {
            if (!Directory.Exists("Assets/Resources/ScriptableObjects/Cards")) {
                Debug.LogWarning("The directory doesn't exist or it has the wrong name! The directory should be: Assets/Resources/ScriptableObjects/Cards");
                return;
            }

            string[] cardsGuids = AssetDatabase.FindAssets("t:CardSO", new[] { "Assets/Resources/ScriptableObjects/Cards" });

            if (cardsGuids.Length == 0)
            {
                Debug.LogWarning("No cards for the selected faction were found, verify if the cards are in the correct folder");
                return;
            }

            CardDict cardDict = new();

            for (int i = 0; i < cardsGuids.Length; i++) 
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(cardsGuids[i]);

                CardSO cardSO = AssetDatabase.LoadAssetAtPath<CardSO>(assetPath);

                CardDictItem cardDictItem = new CardDictItem { Id = cardSO.Id, ItemCardSO = cardSO };

                cardDict.DictItems.Add(cardDictItem);
            }

            dictionary.UpdateDictionary(cardDict);

            Debug.Log($"Updated the Card Dictionary. Found a total of {cardsGuids.Length} cards!");
        }
    }
#endif
}
