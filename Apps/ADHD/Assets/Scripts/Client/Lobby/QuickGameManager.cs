using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartQueueSearch : MonoBehaviour
{

    private bool isInQueue = false;

    public void FindGame()
    {
        if (isInQueue) { return; }
        RelayManager.Singleton.JoinQueue();
        isInQueue = true;
    }
}
