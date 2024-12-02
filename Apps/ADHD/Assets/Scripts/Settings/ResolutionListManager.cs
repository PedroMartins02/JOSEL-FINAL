using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionListManager : MonoBehaviour
{
  [Header("Managers")]
  public VideoSettingUpdater videoSettings;

  [Header("List Generation")]
  public GameObject resolutionItemPrefab;
  public Transform listContainer;

  private Resolution[] resolutions;

  public void PopulateResolutionList()
  {
	resolutions = Screen.resolutions;
	foreach (Resolution res in resolutions)
	{
	  // check if resolution is valid
	  string resolutionStr = $"{res.width}x{res.height}";
	  if (!SettingsManager.Instance.resolutionSet.Contains(resolutionStr))
		continue;

	  GameObject resolutionItem = Instantiate(resolutionItemPrefab, listContainer);

	  Image image = resolutionItem.GetComponentInChildren<Image>();
	  if (image != null)
	  {
		TextMeshProUGUI resolutionText = resolutionItem.GetComponentInChildren<TextMeshProUGUI>();
		if (resolutionText != null)
		  resolutionText.text = resolutionStr;
	  }

	  Button resolutionButton = resolutionItem.GetComponentInChildren<Button>();
	  if (resolutionButton != null)
	  {
		if (res.Equals(Screen.currentResolution))
		  resolutionButton.Select();
		resolutionButton.onClick.AddListener(() => OnResolutionSelected(resolutionButton, res));
	  }
	}
  }

  private void OnResolutionSelected(Button btn, Resolution res)
  {
	int resolutionIndex = -1;
	for (int i = 0; i < resolutions.Length; i++)
	  if (res.Equals(resolutions[i])) resolutionIndex = i;

	if (resolutionIndex == -1)
	  return;

	btn.Select();
	videoSettings.ChangeResolution(resolutionIndex);
	Debug.Log(resolutions[resolutionIndex].ToString() + " selected (index: " + resolutionIndex + ")");
  }
}
