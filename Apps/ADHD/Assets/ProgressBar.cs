using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif

    public Container container; // Reference to the Container ScriptableObject
    public Image fill;
    public Color color;

    void Start()
    {
        UpdateProgress();
    }

    void Update()
    {
        UpdateProgress();
    }

    void UpdateProgress()
    {
        if (container != null)
        {
            float fillAmount = (float)container.progress * container.scale;
            fill.fillAmount = fillAmount;
            fill.color = color;
        }
    }
}
