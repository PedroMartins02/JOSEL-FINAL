using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
  public static SettingsManager Instance;

  [Header("Settings")]
  public bool isFullScreen;
  public int resolutionIndex;
  public int masterVolumeIndex;
  public int musicVolumeIndex;
  public int sfxVolumeIndex;

  private void Awake()
  {
	if (Instance == null)
	{
	  Instance = this;
	  DontDestroyOnLoad(gameObject);
	  LoadSettings();
	}
	else
	{
	  Destroy(gameObject);
	}
  }

  public void LoadSettings()
  {
	isFullScreen = PlayerPrefs.GetInt("Settings FullScreen", 1) == 1;
	resolutionIndex = PlayerPrefs.GetInt("Settings Resolution", 0);
	masterVolumeIndex = PlayerPrefs.GetInt("Settings Master Volume", 10);
	musicVolumeIndex = PlayerPrefs.GetInt("Settings Music Volume", 10);
	sfxVolumeIndex = PlayerPrefs.GetInt("Settings SFX Volume", 10);

	ApplySettings();
  }

  public void SaveSettings()
  {
	Debug.Log("Saving settings...");
	PlayerPrefs.SetInt("Settings FullScreen", isFullScreen ? 1 : 0);
	PlayerPrefs.SetInt("Settings Resolution", resolutionIndex);
	PlayerPrefs.SetInt("Settings Master Volume", masterVolumeIndex);
	PlayerPrefs.SetInt("Settings Music Volume", musicVolumeIndex);
	PlayerPrefs.SetInt("Settings SFX Volume", sfxVolumeIndex);
	PlayerPrefs.Save();
  }
  
  public void ApplySettings()
  {
	Debug.Log("Applying settings");
	// Set Resolution for desktop environments
	Screen.fullScreen = isFullScreen;
	Resolution[] resolutions = Screen.resolutions;

	if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
	{
	  Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, isFullScreen);
	}

	// Set game Volumes
  }
}
