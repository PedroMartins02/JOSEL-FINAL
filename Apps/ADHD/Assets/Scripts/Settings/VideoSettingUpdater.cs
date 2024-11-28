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

  private Resolution[] resolutions;

  void Start()
  {
	resolutions = Screen.resolutions;
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
	resolutionIndex = SettingsManager.Instance.resolutionIndex;
	isFullscreen = SettingsManager.Instance.isFullScreen;

	resolutionText.text = $"{resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}";
	displayText.text = isFullscreen ? "Fullscreen" : "Windowed";
  }

  public void ChangeResolution(int resolutionIndex)
  {
	this.resolutionIndex = resolutionIndex;
	resolutionText.text = $"{resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}";
  }

  public void ChangeDisplay(bool fullscreen)
  {
	isFullscreen = fullscreen;
	displayText.text = isFullscreen ? "Fullscreen" : "Windowed";
  }
}