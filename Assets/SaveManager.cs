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



    public void SaveClothesSet(string key, ClothesConfig clothesConf)
    {
        changableDataConfig.AddClothesConfig(key, clothesConf);
        SaveChangableConfig();
    }

    public ClothesConfig LoadClothesSet(string key)
    {
        LoadChangableConfig();
        return changableDataConfig?.GetClothesConfig(key);
    }

    public int GetLvl()
    {
        LoadImportantConfig();
        return importantDataConfig.lvl;

    }

    public int GetExp()
    {
        LoadImportantConfig();
        return importantDataConfig.exp;
    }

    public void SaveLvl(int Lvl)
    {
        importantDataConfig.lvl = Lvl;
        SaveImportantConfig();
    }

    public void SaveExp(int exp)
    {
        importantDataConfig.exp = exp;
        SaveImportantConfig();
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
        string key = savePrefix + config.ToString();
        var json = JsonUtility.ToJson(config);
        PlayerPrefs.SetString(key, json);
    }
    private void SaveShopConfig()
    {
        SaveConfig(shopDataConfig);
    }

    private void SaveImportantConfig()
    {
        SaveConfig(importantDataConfig);
    }

    private void SaveChangableConfig()
    {
        SaveConfig(changableDataConfig);
    }

    private T LoadConfig<T>(T cfg) where T : IDataConfig
    {

        string key = savePrefix + cfg?.ToString();
        var json = PlayerPrefs.GetString(key);
        var CONFIG = JsonUtility.FromJson<T>(json);
        if (CONFIG != null)
        {
            return CONFIG;
        }
        return default;
    }

    private void LoadShopConfig()
    {
        shopDataConfig = LoadConfig(shopDataConfig);
    }

    private void LoadImportantConfig()
    {
        importantDataConfig = LoadConfig(importantDataConfig);
    }

    private void LoadChangableConfig()
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
}
