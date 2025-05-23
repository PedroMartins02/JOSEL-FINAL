using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButtonManager : MonoBehaviour
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
  }
}
