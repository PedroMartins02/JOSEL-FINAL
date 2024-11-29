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
  public int itemSpacing;

  private Resolution[] resolutions;

  void Start()
  {
	resolutions = Screen.resolutions;
	PopulateResolutionList();
  }

  private void PopulateResolutionList()
  {
	int currentSpacing = 0;
	foreach (Resolution res in resolutions)
	{
	  GameObject resolutionItem = Instantiate(resolutionItemPrefab, listContainer);

	  RectTransform rectTransform = resolutionItem.GetComponent<RectTransform>();
	  if (rectTransform != null)
	  {
		rectTransform.anchoredPosition = new Vector2(0, currentSpacing);
		currentSpacing -= itemSpacing; // Move down by prefab height + spacing
	  }

	  Image image = resolutionItem.GetComponentInChildren<Image>();
	  if (image != null)
	  {
		TextMeshProUGUI resolutionText = resolutionItem.GetComponentInChildren<TextMeshProUGUI>();
		if (resolutionText != null)
		  resolutionText.text = $"{res.width}x{res.height}";
		else
		  Debug.Log("Text not found in resolutionItem");
	  }
	  else
		Debug.Log("Image not found in resolutionItem");

	  Button resolutionButton = resolutionItem.GetComponentInChildren<Button>();
	  if (resolutionButton != null)
	  {
		if (res.Equals(Screen.currentResolution))
		  resolutionButton.Select();
		resolutionButton.onClick.AddListener(() => OnResolutionSelected(resolutionButton, res));
	  }
	  else
		Debug.Log("Button not found in resolutionItem");
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
	//videoSettings.ChangeResolution(resolutionIndex);
	Debug.Log(resolutions[resolutionIndex].ToString() + " selected (index: " + resolutionIndex + ")");
  }
}
