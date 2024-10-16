using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
  [SerializeField] private Transform contentView;

  [SerializeField] private GameObject generalSettingPrefab;
  [SerializeField] private GameObject videoSettingPrefab;
  [SerializeField] private GameObject audioSettingPrefab;
  [SerializeField] private GameObject socialSettingPrefab;

  void Start()
  {
    NavigateToGeneralSection();
  }
    
  public void NavigateToGeneralSection()
  {
    ClearContentView();
    Instantiate(generalSettingPrefab, contentView);
  }

  public void NavigateToVideoSection()
  {
    ClearContentView();
    Instantiate(videoSettingPrefab, contentView);
  }

  public void NavigateToAudioSection()
  {
    ClearContentView();
    Instantiate(audioSettingPrefab, contentView);
  }

  public void NavigateToSocialSection()
  {
    ClearContentView();
    Instantiate(socialSettingPrefab, contentView);
  }

  private void ClearContentView()
  {
    foreach (Transform child in contentView)
    {
      Destroy(child.gameObject);
    }
  }
}
