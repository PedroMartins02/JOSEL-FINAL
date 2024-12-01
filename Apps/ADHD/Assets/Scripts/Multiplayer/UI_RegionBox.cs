using Newtonsoft.Json.Linq;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_RegionBox : MonoBehaviour
{
    [SerializeField] private Image elementImage;

    private const string GeoLocationAPI = "https://ipinfo.io/json";

    private string region;

    private void Start()
    {
        StartCoroutine(FetchPlayerRegion());
    }

    private IEnumerator FetchPlayerRegion()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GeoLocationAPI))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching region data: {webRequest.error}");
                yield break;
            }

            string jsonResponse = webRequest.downloadHandler.text;
            Debug.Log(jsonResponse);
            string region = ExtractRegionFromJson(jsonResponse);

            Debug.Log(region);
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
                        string region = values[1].Replace("\"", string.Empty);
                        this.region = region;
                        return region;
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
                        string region = values[1].Replace("\"", string.Empty);
                        this.region = region;
                        return region;
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
