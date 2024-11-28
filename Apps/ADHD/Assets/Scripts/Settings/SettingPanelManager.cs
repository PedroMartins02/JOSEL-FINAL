using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelManager : MonoBehaviour
{
  [Header("Side Buttons")]
  public Button generalButton;
  public Button videoButton;

  public void Start()
  {
    generalButton.interactable = false;
    if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
    {
      videoButton.interactable = false;
    }

	  Debug.Log("platform: " + Application.platform.ToString());
  }
}