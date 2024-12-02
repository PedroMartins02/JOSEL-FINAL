using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
  public static SettingsManager Instance;
  public AudioMixerManager audioMixerManager;
  public HashSet<string> resolutionSet;

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
	  SetResolutions();
	}
	else
	{
	  Destroy(gameObject);
	}
  }

  public void LoadSettings()
  {
	isFullScreen = PlayerPrefs.GetInt("Settings FullScreen", 1) == 1;
	resolutionIndex = PlayerPrefs.GetInt("Settings Resolution", GetCurrentResolutionIndex());
	masterVolumeIndex = PlayerPrefs.GetInt("Settings Master Volume", 10);
	musicVolumeIndex = PlayerPrefs.GetInt("Settings Music Volume", 10);
	sfxVolumeIndex = PlayerPrefs.GetInt("Settings SFX Volume", 10);

	ApplySettings();
  }

  public void SaveSettings()
  {
	PlayerPrefs.SetInt("Settings FullScreen", isFullScreen ? 1 : 0);
	PlayerPrefs.SetInt("Settings Resolution", resolutionIndex);
	PlayerPrefs.SetInt("Settings Master Volume", masterVolumeIndex);
	PlayerPrefs.SetInt("Settings Music Volume", musicVolumeIndex);
	PlayerPrefs.SetInt("Settings SFX Volume", sfxVolumeIndex);
	PlayerPrefs.Save();
  }

  public void ApplySettings()
  {
	audioMixerManager.UpdateVolumeLevels();

	Resolution[] resolutions = Screen.resolutions;
	if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
	{
	  resolutionIndex = resolutions.Length - 1;
	}

	FullScreenMode displayMode;
	if (isFullScreen)
	  displayMode = FullScreenMode.ExclusiveFullScreen;
	else
	  displayMode = FullScreenMode.Windowed;

	Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, displayMode);
	Debug.Log("Resolution set!");
  }

  public int GetCurrentResolutionIndex()
  {
	Resolution[] resolutions = Screen.resolutions;
	for (int i = 0; i < resolutions.Length; i++)
	{
	  if (resolutions[i].Equals(Screen.currentResolution))
		return i;
	}
	return -1;
  }

  private void SetResolutions()
  {
	resolutionSet = new HashSet<string> { "1920x1080", "1600x900", "1366x768", "1280x720" };
  }
}