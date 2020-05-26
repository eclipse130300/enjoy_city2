using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager> 
{
    public ShopDataConfig shopDataConfig;
    public ImportantDataConfig importantDataConfig;
    public ChangableDataConfig changableDataConfig;
    public string savePrefix = "save_";
    private void Awake()
    {
        LoadAllConfigs();
    }

    public void LoadAllConfigs()
    {
        LoadShopConfig();
        LoadImportantConfig();
        LoadChangableConfig();
    }

    private void SaveConfig(IDataConfig config)
    {
        Debug.Log(config.ToString());
        string key = config.ToString();
        var json = JsonUtility.ToJson(config);
        PlayerPrefs.SetString(key, json);
    }
    public void SaveShopConfig()
    {
        SaveConfig(shopDataConfig);
    }

    public void SaveImportantConfig()
    {
        SaveConfig(importantDataConfig);
    }

    public void SaveChangableConfig()
    {
        SaveConfig(changableDataConfig);
    }

    public T LoadConfig<T>(T cfg) where T : IDataConfig
    {

        string key = cfg.ToString();
        var json = PlayerPrefs.GetString(key);
        var CONFIG = JsonUtility.FromJson<T>(json);
        return CONFIG;
    }

    public void LoadShopConfig()
    {
        shopDataConfig = LoadConfig(shopDataConfig);
    }

    public void LoadImportantConfig()
    {
        importantDataConfig = LoadConfig(importantDataConfig);
    }

    public void LoadChangableConfig()
    {
        changableDataConfig = LoadConfig(changableDataConfig);
    }

    private void OnApplicationQuit()
    {
        SaveAllConfigs();
    }

    private void SaveAllConfigs()
    {
        SaveChangableConfig();
        SaveImportantConfig();
        SaveShopConfig();
    }

    public T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public string ToJson<T>(T obj)
    {
        return JsonUtility.ToJson(obj);
    }

    private void Update()
    {

    }
}
