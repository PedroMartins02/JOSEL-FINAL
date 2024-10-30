using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameModel
{
    public class CardSO : ScriptableObject
    {
        [Header("General Info")]
        public string Name;
        public string ShortText;
        [TextArea] public string Description;
        public Sprite Art;

        [Space]
        [Header("Faction & Element")]
        public Factions Faction;
        public Elements Element;

        [Space]
        [Header("Stats")]
        public int Blessings;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(CardSO))]
    public class CardSOEditor : Editor
    {
        protected CardSO card;

        private void OnEnable()
        {
            card = target as CardSO;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (card == null)
                return;

            SerializedProperty prop = serializedObject.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                EditorGUILayout.PropertyField(prop, true);

                if (prop.name == "Art")
                {
                    GUILayout.Space(5);

                    if (card.Art != null)
                    {
                        Texture2D texture = AssetPreview.GetAssetPreview(card.Art);

                        Rect spriteRect = card.Art.rect;

                        float aspect = spriteRect.width / spriteRect.height;

                        float previewWidth = 150;
                        float previewHeight = previewWidth / aspect;

                        if (texture != null)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(texture, GUILayout.Height(previewHeight), GUILayout.Width(previewWidth));
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}