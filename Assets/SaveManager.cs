using CMS.Config;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;

public class SaveManager : Singleton<SaveManager> //TODO inherit from baseGameManager -- 4 errors now!
{
    static ShopDataConfig shopDataConfig = new ShopDataConfig();
    static ImportantDataConfig importantDataConfig = new ImportantDataConfig();
    static ChangableDataConfig changableDataConfig = new ChangableDataConfig();
    static string savePrefix = "save_";
    private void Awake()
    {
        LoadAllConfigs();
    }
#if UNITY_EDITOR
    #region DEBUG
    [MenuItem("DEBUG/DELETE !ALL! CONFIGS")]
    static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("ALL! CONFIGS DELETED!");
    }
    [MenuItem("DEBUG/DELETE BOUGHT ITEMS(ROOM, SKINS)")]
    static void DeleteShopDataConfig()
    {
        PlayerPrefs.DeleteKey(savePrefix + shopDataConfig.ToString());
        Debug.Log("BOUGHT ITEMS DELETED!");
    }

    [MenuItem("DEBUG/DELETE IMPORTANT DATA(EXP, LVL, MONEY)")]
    static void DeleteImportantDataConfig()
    {
        PlayerPrefs.DeleteKey(savePrefix + importantDataConfig.ToString());
        Debug.Log("IMPORTANT DATA DELETED!");
    }

    [MenuItem("DEBUG/DELETE CHANGABLE DATA(NICK, CURRENT SKINS, GENDER)")]
    static void DeleteChangableDataConfig()
    {
        PlayerPrefs.DeleteKey(savePrefix + changableDataConfig.ToString());
        Debug.Log("CHANGABLE DATA DELETED!");
    }
    #endregion
#endif

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

    public int GetSoftCurrency()
    {
        return importantDataConfig.softCurrency;
    }

    public int GetHardCurrency()
    {
        return importantDataConfig.hardCurrency;
    }

    public void SetSoftCurrency(int amount)
    {
        importantDataConfig.softCurrency = amount;
    }

    public bool CheckIsEnoughMoney(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.SOFT:
                if (GetSoftCurrency() >= amount)
                {
                    return true;
                }
                break;
            case CurrencyType.HARD:
                if (GetHardCurrency() >= amount)
                {
                    return true;
                }
                break;
        }
        return false;
    }
    //save manager buys?
    public void Buy(ItemConfig cfg, ItemVariant varitant, int cost, CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.SOFT:
                importantDataConfig.softCurrency -= cost;
                break;
            case CurrencyType.HARD:
                importantDataConfig.hardCurrency -= cost;
                break;
        }
        shopDataConfig.AddItemToBoughtList(cfg, varitant);
    }

    public void SetHardCurrency(int amount)
    {
        importantDataConfig.hardCurrency = amount;
    }

    public void Add3DItemToShopList(ItemConfig conf, ItemVariant activeVar)
    {

    }

    public int GetExp()
    {
        LoadImportantConfig();
        return importantDataConfig.exp;
    }

    public int GetExpToNextLevel()
    {
        LoadImportantConfig();
        return importantDataConfig.expToNextLvl;
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

    public void SaveExpToNextLvl(int expToNLVL)
    {
        importantDataConfig.expToNextLvl = expToNLVL;
        SaveImportantConfig();
    }


    #region FIELD_SERIALIZATION

    private void SaveAllConfigs()
    {
        SaveChangableConfig();
        SaveImportantConfig();
        SaveShopConfig();
    }

    private void SaveConfig(IDataConfig config)
    {
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

    #endregion

    #region FIELD_DESERIALIZATION

    public void LoadAllConfigs()
    {
        LoadShopConfig();
        LoadImportantConfig();
        LoadChangableConfig();
    }

    private T LoadConfig<T>(T cfg) where T : IDataConfig, new()
    {

        string key = savePrefix + cfg?.ToString();
        if(PlayerPrefs.HasKey(key))
        {
            var json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<T>(json);
        }
           return new T();  
    }

    private void LoadShopConfig()
    {
        shopDataConfig = LoadConfig(shopDataConfig);
    }

    private void LoadImportantConfig()
    {
        if (LoadConfig(importantDataConfig) == null)
        {
            importantDataConfig = new ImportantDataConfig();
        }   
        importantDataConfig = LoadConfig(importantDataConfig);
    }

    private void LoadChangableConfig()
    {
        changableDataConfig = LoadConfig(changableDataConfig);
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveAllConfigs();
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
