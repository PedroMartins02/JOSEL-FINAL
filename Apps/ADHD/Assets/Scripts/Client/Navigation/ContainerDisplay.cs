using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContainerDisplay : MonoBehaviour
{
    [SerializeField] public Container container;

    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] public Image backgroundImage;
    [SerializeField] public TextMeshProUGUI taskcompletionText;
    [SerializeField] public TextMeshProUGUI idText;
    [SerializeField] public TextMeshProUGUI coinsText;
    void Start()
    {
        //nameText.text = container.name;
        descriptionText.text = container.description;
       // backgroundImage.sprite = container.background;
        taskcompletionText.text = container.task_completion;
       // idText.text = container.id.ToString();
        coinsText.text = container.coins.ToString();
    }

}
