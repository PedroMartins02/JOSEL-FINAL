using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
  [Header("Sliders")]
  public Slider[] volumeSliders;

  private int masterVol, musicVol, sfxVol;

  private void Start()
  {
	SetBaseAudioSettings();
  }

  public void ChangeMasterVolume()
  {
	masterVol = (int) volumeSliders[0].value;
  }

  public void ChangeMusicVolume()
  {
	musicVol = (int) volumeSliders[1].value;
  }

  public void ChangeSFXVolume()
  {
	sfxVol = (int) volumeSliders[2].value;
  }

  public void SetBaseAudioSettings()
  {
	masterVol = SettingsManager.Instance.masterVolumeIndex;
	musicVol = SettingsManager.Instance.musicVolumeIndex;
	sfxVol = SettingsManager.Instance.sfxVolumeIndex;

	volumeSliders[0].value = masterVol;
	volumeSliders[1].value = musicVol;
	volumeSliders[2].value = sfxVol;
  }

  public void SaveAudioSettings()
  {
	SettingsManager.Instance.masterVolumeIndex = masterVol;
	SettingsManager.Instance.musicVolumeIndex = musicVol;
	SettingsManager.Instance.sfxVolumeIndex = sfxVol;
	SettingsManager.Instance.SaveSettings();
	SettingsManager.Instance.ApplySettings();

	// TODO: modify the volume values
  }
}
