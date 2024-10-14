using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartQueueSearch : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(QueueUp);
    }

    void QueueUp()
    {
        RelayManager.Singleton.JoinQueue();
    }
}
