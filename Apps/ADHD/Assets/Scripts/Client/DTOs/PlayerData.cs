using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModel;

[System.Serializable]
public class PlayerData
{
    public string Name { get; set; } = "PlayerName";
    public List<string> CardCollection { get; set; } = new List<string>();
    public List<DeckData> DeckCollection { get; set; } = new List<DeckData>();
    public int SelectedDeckId { get; set; } = 0;
    public List<int> CardBackCollection { get; set; } = new List<int>();
    public int Tokens { get; set; } = 0;
    public int MMR { get; set; } = 400;
    public List<bool> MatchHistory { get; set; } = new List<bool>();

    // Parameterless constructor for JSON deserialization
    public PlayerData()
    {
    }

    public PlayerData(string name)
    {
        Name = name;
        CardCollection = new List<string>();
        DeckCollection = new List<DeckData>();
        SelectedDeckId = 0;
        CardBackCollection = new List<int>();
        Tokens = 0;
        MMR = 400;
        MatchHistory = new List<bool>();
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
