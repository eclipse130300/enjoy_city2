using CMS.Config;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;

public class SaveManager : Singleton<SaveManager> //TODO inherit from baseGameManager -- 4 errors now!
{
    public ShopDataConfig shopDataConfig = new ShopDataConfig();
    public ImportantDataConfig importantDataConfig = new ImportantDataConfig();
    public ChangableDataConfig changableDataConfig = new ChangableDataConfig();
    static string savePrefix = "save_";
    private void Awake()
    {
        LoadAllConfigs();

     //  Loader.Instance.AllSceneLoaded += SaveAllConfigs;
    }

    public override void OnDestroy()
    {
     //   Loader.Instance.AllSceneLoaded -= SaveAllConfigs;
    }
#if UNITY_EDITOR
    #region DEBUG
    [MenuItem("DEBUG/DELETE !ALL! CONFIGS")]
    static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("ALL! CONFIGS DELETED!");
    }
/*    [MenuItem("DEBUG/DELETE BOUGHT ITEMS(ROOM, SKINS)")]
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
    }*/
    #endregion
#endif

    public ItemVariant GetActiveVariant(RoomItemConfig cfg)
    {
        return LoadRoomSet().GetActiveVariant(cfg);
    }

    public void GetActiveVariant(ItemConfig cfg)
    {
        //todo think about this shit
    }

    public List<string> Get3DItemList()
    {
        return shopDataConfig.bought3DModelItems;
    }

    public List<string> GetRoomItemList()
    {
        return shopDataConfig.boughtRoomItems;
    }

    public void SaveClothesSet(string key, ClothesConfig clothesConf)
    {
        changableDataConfig.AddClothesConfig(key, clothesConf);
        SaveChangableConfig();
    }

    public ClothesConfig LoadClothesSet(string key)
    {
        var clothes = changableDataConfig?.GetClothesConfig(key);
        return clothes;
    }

    public void SaveRoomSet(RoomConfig roomCFG)
    {
        changableDataConfig.roomConfig = roomCFG;
        SaveAllConfigs();
    }

    public RoomConfig LoadRoomSet()
    {
        var roomCFG = changableDataConfig.roomConfig;
        return roomCFG;

    }
    public int GetLvl()
    {
       // LoadAllConfigs();
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

    public void SetHardCurrency(int amount)
    {
        importantDataConfig.hardCurrency = amount;
    }

    public void Add3DItemToShopList(ItemConfig conf, ItemVariant activeVar)
    {
        var list = shopDataConfig.bought3DModelItems;
        var pair = string.Concat(conf.ConfigId, '+', activeVar.ConfigId);

        if(list.Contains(pair)) { return; }
        list.Add(pair);
    }

    public void AddRoomItemToShopList(RoomItemConfig conf, ItemVariant activeVar)
    {
        var list = shopDataConfig.boughtRoomItems;
        var pair = string.Concat(conf.ConfigId, '+', activeVar.ConfigId);

        if (list.Contains(pair)) { return; }
        list.Add(pair);
    }

    public int GetExp()
    {
      //  LoadAllConfigs();
        return importantDataConfig.exp;
    }

    public int GetExpToNextLevel()
    {
     //   LoadAllConfigs();
        return importantDataConfig.expToNextLvl;
    }

    public void SaveLevelData(int exp, int expTonextLvl, int Lvl)
    {
        importantDataConfig.exp = exp;
        importantDataConfig.lvl = Lvl;
        importantDataConfig.expToNextLvl = expTonextLvl;
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
