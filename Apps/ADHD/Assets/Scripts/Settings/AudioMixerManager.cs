using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
  public AudioMixer audioMixer;

  private void Start()
  {
	UpdateVolumeLevels();
  }

  public void UpdateVolumeLevels()
  {
	float masterVolume = MapVolumeToDecibels(SettingsManager.Instance.masterVolumeIndex);
	float musicVolume = MapVolumeToDecibels(SettingsManager.Instance.musicVolumeIndex);
	float sfxVolume = MapVolumeToDecibels(SettingsManager.Instance.sfxVolumeIndex);

	audioMixer.SetFloat("MasterVolume", masterVolume);
	audioMixer.SetFloat("MusicVolume", musicVolume);
	audioMixer.SetFloat("SFXVolume", sfxVolume);
  }

  public float MapVolumeToDecibels(int volume)
  {
	return volume == 0 ? -80f : Mathf.Lerp(-30f, 0f, volume / 10f);
  }
}
