using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UI_RegionBox : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI regionText;

    private const string GeoLocationAPI = "https://ipinfo.io/json";

    private void Start()
    {
        StartCoroutine(FetchPlayerRegion());
    }

    private IEnumerator FetchPlayerRegion()
    {
        if (regionText == null)
        {
            Debug.LogError("Region Text is not assigned.");
            yield break;
        }

        // Using UnityWebRequest to fetch data from ipinfo.io
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GeoLocationAPI))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching region data: {webRequest.error}");
                regionText.text = "Error";
                yield break;
            }

            // We need to parse the JSON response to get the region dumahh
            string jsonResponse = webRequest.downloadHandler.text;
            Debug.Log(jsonResponse);
            string region = ExtractRegionFromJson(jsonResponse);

            Debug.Log(region);
            regionText.text = string.IsNullOrEmpty(region) ? "Region: Unknown" : $"{region}";
        }
    }

    private string ExtractRegionFromJson(string json)
    {
        try
        {
            // Extract the region using simple string manipulation
            if (json.Contains("\"region\""))
            {
                string[] loc = json.Split(',');
                foreach (string option in loc)
                {
                    if (!string.IsNullOrWhiteSpace(option) && option.Contains("\"region\""))
                    {
                        string[] values = option.Split(':');
                        return values[1];
                    }
                }
            }

            // if not region then country
            if (json.Contains("\"country\""))
            {
                string[] loc = json.Split(',');
                foreach (string option in loc)
                {
                    if (!string.IsNullOrWhiteSpace(option) && option.Contains("\"country\""))
                    {
                        string[] values = option.Split(':');
                        return values[1];
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("Failed to parse region from JSON.");
        }

        return null;
    }
}
