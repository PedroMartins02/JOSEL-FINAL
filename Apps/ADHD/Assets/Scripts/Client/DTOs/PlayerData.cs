using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;

[System.Serializable]
public class PlayerData
{
    public string Name { get; set; }
    public Dictionary<string, int> CardCollection { get; set; } = new Dictionary<string, int>();
    public List<DeckData> DeckCollection { get; set; } = new List<DeckData>();

    // Parameterless constructor for JSON deserialization
    public PlayerData()
    {
    }

    public PlayerData(string name)
    {
        Name = name;
        CardCollection = new Dictionary<string, int>();
        DeckCollection = new List<DeckData>();
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static PlayerData FromJson(string jsonData)
    {
        return JsonConvert.DeserializeObject<PlayerData>(jsonData);
    }

    public Dictionary<string, object> ToDictionary()
    {
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(this.ToJson());
    }

    public static PlayerData FromDictionary(Dictionary<string, object> dict)
    {
        return JsonConvert.DeserializeObject<PlayerData>(JsonConvert.SerializeObject(dict));
    }
}
