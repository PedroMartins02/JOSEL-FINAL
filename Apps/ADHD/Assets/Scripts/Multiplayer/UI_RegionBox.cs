using Newtonsoft.Json.Linq;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_RegionBox : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    [Header("ImageReferences")]
    [SerializeField] private Image elementImage;
    [SerializeField] private Image lightDarkImage;

    [Header("ElementSprites")]
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite earthSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite airSprite;

    [Header("LightDarkSprites")]
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    private const string GeoLocationAPI = "https://ipinfo.io/json";
    private const string WeatherAPI = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";

    private string region;
    private string apiKey = "d44413290154979bc5852bfc9b5b9aff";

    private void Start()
    {
        StartLoading();
        elementImage.enabled = false;
        lightDarkImage.enabled = false;
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

            if (!string.IsNullOrEmpty(region))
            {
                StartCoroutine(FetchWeatherData(region));
            } else
            {
                HideSelf();
            }
        }
    }

    private IEnumerator FetchWeatherData(string city)
    {
        string weatherUrl = string.Format(WeatherAPI, city, apiKey);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(weatherUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching weather data: {webRequest.error}");
                yield break;
            }

            string jsonResponse = webRequest.downloadHandler.text;
            Debug.Log(jsonResponse);
            ApplyWeatherAndTimeData(jsonResponse);
        }
    }

    private void ApplyWeatherAndTimeData(string weatherJson)
    {
        try
        {
            JObject weatherData = JObject.Parse(weatherJson);
            string weatherMain = weatherData["weather"][0]["main"].ToString();
            float temperature = float.Parse(weatherData["main"]["temp"].ToString());
            long sunrise = long.Parse(weatherData["sys"]["sunrise"].ToString());
            long sunset = long.Parse(weatherData["sys"]["sunset"].ToString());
            long currentTime = long.Parse(weatherData["dt"].ToString());

            if (temperature > 27) {
                elementImage.sprite = fireSprite;
                AccountManager.Singleton.WeatherElement = GameModel.Elements.Fire;
            }
            else if (weatherMain.Contains("Rain"))
            {
                elementImage.sprite = waterSprite;
                AccountManager.Singleton.WeatherElement = GameModel.Elements.Water;
            }
            else if (weatherMain.Contains("Wind"))
            {
                elementImage.sprite = airSprite;
                AccountManager.Singleton.WeatherElement = GameModel.Elements.Air;
            }
            else
            {
                elementImage.sprite = earthSprite;
                AccountManager.Singleton.WeatherElement = GameModel.Elements.Earth;
            }

            lightDarkImage.sprite = currentTime >= sunrise && currentTime <= sunset ? lightSprite : darkSprite;
            AccountManager.Singleton.TimeElement = currentTime >= sunrise && currentTime <= sunset ? GameModel.Elements.Light : GameModel.Elements.Dark;

            elementImage.enabled = true;
            lightDarkImage.enabled = true;

            StopLoading();
        }
        catch
        {
            Debug.LogWarning("Failed to parse weather or time data.");

            HideSelf();
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

    private void HideSelf()
    {
        StopLoading();
        gameObject.SetActive(false);
    }

    private void StartLoading()
    {
        loadingScreen.SetActive(true);
    }

    private void StopLoading()
    {
        loadingScreen.SetActive(false);
    }
}
