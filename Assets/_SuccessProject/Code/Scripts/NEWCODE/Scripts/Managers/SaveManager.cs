using CMS.Config;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SaveManager : MonoBehaviourSingleton<SaveManager>
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
        SaveAllConfigs();
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

    #endregion
#endif
    public void DeleteAllConfigs()
    {
        shopDataConfig = new ShopDataConfig();
        importantDataConfig = new ImportantDataConfig();
        changableDataConfig = new ChangableDataConfig();
        PlayerPrefs.DeleteAll();
        Debug.Log("ALL! CONFIGS DELETED!");
    }

    public Sprite GetBodySprite()
    {
        var body = LoadBody();
        return body.bodyIcon;
    }

    public string GetNickName()
    {
       return changableDataConfig.GetNickName();
    }

    public void SetNick(string nick)
    {
        changableDataConfig.SetNickName(nick);
    }

    public BodyConfig LoadBody()
    {
        BodyConfig body = ScriptableList<BodyConfig>.instance.GetItemByID(changableDataConfig.currentBodyConfigId);

        return body;
    }

    public void SaveBody(BodyConfig bodyConfig)
    {
        changableDataConfig.currentBodyConfigId = bodyConfig.ConfigId;
    }


    public ItemVariant GetActiveVariant(RoomItemConfig cfg)
    {
        return LoadRoomSet().GetActiveVariant(cfg);
    }

/*    public void GetActiveVariant(ItemConfig cfg)
    {
        //todo think about this shit
    }*/

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

    public ClothesConfig LoadClothesSet(string key) //key is gender + gameMode
    {
        var clothes = changableDataConfig?.GetClothesConfig(key);
        return clothes;
    }

    public void SaveRoomSet(RoomConfig roomCFG)
    {
        changableDataConfig.roomConfig = roomCFG;
        SaveChangableConfig();
    }

    public RoomConfig LoadRoomSet()
    {
        var roomCFG = changableDataConfig.roomConfig;
        return roomCFG;

    }
    public int GetLvl()
    {
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

    private void SetSoftCurrency(int amount)
    {
        importantDataConfig.softCurrency = amount;
        Messenger.Broadcast(GameEvents.CURRENCY_UPDATED);
    }

    private void SetHardCurrency(int amount)
    {
        importantDataConfig.hardCurrency = amount;
        Messenger.Broadcast(GameEvents.CURRENCY_UPDATED);
    }

    public void AddSoftCurrency(int amount)
    {
        int currentSoft = GetSoftCurrency();
        currentSoft += amount;
        SetSoftCurrency(currentSoft);
    }

    public void AddHardCurrency(int amount)
    {
        int currentHard = GetHardCurrency();
        currentHard += amount;
        SetHardCurrency(currentHard);
    }

    public void SpendSoftCurrency(int amount)
    {
        int currentSoft = GetSoftCurrency();
        currentSoft -= amount;
        SetSoftCurrency(currentSoft);
    }

    public void SpendHardCurrency(int amount)
    {
        int currentHard = GetHardCurrency();
        currentHard -= amount;
        SetHardCurrency(currentHard);
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
