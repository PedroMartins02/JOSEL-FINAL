using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingUpdater : MonoBehaviour
{
  [Header("Update Fields")]
  public TextMeshProUGUI resolutionText;
  public TextMeshProUGUI displayText;

  [Header("Radio Buttons")]
  public Button[] resolutionButtons;
  public Button[] displayButtons;

  [HideInInspector]
  public int resolutionIndex;
  [HideInInspector]
  public bool isFullscreen;

  void Start()
  {
	  SetBaseVideoSettings();
  }
  
  public void SaveVideoSettings()
  {
    SettingsManager.Instance.resolutionIndex = resolutionIndex;
    SettingsManager.Instance.isFullScreen = isFullscreen;
    SettingsManager.Instance.SaveSettings();
    SettingsManager.Instance.ApplySettings();
  }
  
  public void SetBaseVideoSettings()
  {
    resolutionText.text = Screen.currentResolution.ToString();
    displayText.text = SettingsManager.Instance.isFullScreen ? "FullScreen" : "Windowed";
    resolutionIndex = SettingsManager.Instance.resolutionIndex;
    isFullscreen = SettingsManager.Instance.isFullScreen;

      Debug.Log("Current Res (by index stored): " + Screen.resolutions[resolutionIndex].ToString());
  }
}