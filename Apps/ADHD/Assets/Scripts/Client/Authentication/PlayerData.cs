using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name { get; set; }

    public PlayerData(string name)
    {
        Name = name;
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
