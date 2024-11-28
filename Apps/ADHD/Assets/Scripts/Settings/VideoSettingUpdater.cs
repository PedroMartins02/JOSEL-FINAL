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
  
  public void SaveAndApplyVideoSettings()
  {
	SettingsManager.Instance.resolutionIndex = resolutionIndex;
	SettingsManager.Instance.isFullScreen = isFullscreen;
	SettingsManager.Instance.SaveSettings();
	SettingsManager.Instance.ApplySettings();
  }
  
  public void SetBaseVideoSettings()
  {
	resolutionIndex = SettingsManager.Instance.GetCurrentResolutionIndex();
	isFullscreen = SettingsManager.Instance.isFullScreen;
	resolutionText.text = Screen.resolutions[resolutionIndex].width + "x" + Screen.resolutions[resolutionIndex].height;
	displayText.text = SettingsManager.Instance.isFullScreen ? "FullScreen" : "Windowed";

	//Debug.Log("Current Res (by index stored): " + Screen.resolutions[resolutionIndex].ToString());
  }

  public void ChangeResolution(int resolutionIndex)
  {
	this.resolutionIndex = resolutionIndex;
	resolutionText.text = Screen.resolutions[resolutionIndex].width + "x" + Screen.resolutions[resolutionIndex].height;
  }

  public void ChangeDisplay(bool fullscreen)
  {
	Debug.Log("Fullscreen? " + isFullscreen);
	isFullscreen = fullscreen;
	displayText.text = isFullscreen? "FullScreen" : "Windowed";
  }
}
