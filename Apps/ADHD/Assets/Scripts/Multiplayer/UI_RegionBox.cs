using Newtonsoft.Json.Linq;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_RegionBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private Image regionImage;

    private const string GeoLocationAPI = "https://ipinfo.io/json";
    private const string PixabayApiKey = "47383260-7b41b142b25998c56668ffc9e";

    private string region;

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

            StartCoroutine(GetRegionImage());
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


    private IEnumerator GetRegionImage()
    {
        string searchQuery = UnityWebRequest.EscapeURL(region + "region");
        string pixabayUrl = $"https://pixabay.com/api/?key={PixabayApiKey}&q={searchQuery}&image_type=photo&orientation=horizontal";

        using (UnityWebRequest request = UnityWebRequest.Get(pixabayUrl))
        {
            yield return request.SendWebRequest();
            Debug.Log(pixabayUrl);
            if (request.result == UnityWebRequest.Result.Success)
            {
                JObject imageData = JObject.Parse(request.downloadHandler.text);
                JArray hits = (JArray)imageData["hits"];

                Debug.Log(hits);

                if (hits != null && hits.Count > 0)
                {
                    if (hits.Count >= 3)
                    {
                        var lowerBound = 0;
                        var upperBound = 2;
                        var rngNum = RandomNumberGenerator.GetInt32(lowerBound, upperBound);

                        string imageUrl = hits[rngNum]["webformatURL"]?.ToString();
                        Debug.Log(imageUrl);
                        StartCoroutine(DownloadImage(imageUrl));
                    }
                    else
                    {
                        string imageUrl = hits[0]["webformatURL"]?.ToString();
                        Debug.Log(imageUrl);
                        StartCoroutine(DownloadImage(imageUrl));
                    }
                }
                else
                {
                    Debug.LogWarning("No images found for the region.");
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch image data: {request.error}");
            }
        }
    }

    private IEnumerator DownloadImage(string imageUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Debug.Log(texture);
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                Debug.Log(sprite);
                if (regionImage)
                {
                    this.regionImage.sprite = sprite;
                }
            }
            else
            {
                Debug.LogError($"Failed to download image: {request.error}");
            }
        }
    }
}
