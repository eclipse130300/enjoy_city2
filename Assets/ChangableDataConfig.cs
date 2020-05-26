using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;
using System.Linq;

[System.Serializable]
public class ChangableDataConfig : ISerializationCallbackReceiver, IDataConfig
{

    public List<ClothesConfig> allConfigs;
    public List<string> configNames = new List<string>();


    string nickName;
    Gender gender;

    public Dictionary<string, ClothesConfig> pickedClothesConfigs = new Dictionary<string, ClothesConfig>(); //нужна тольк во время выполнения программы
                                                                                                        //для связи шмотки и айди активного варианта

    public void AdditemToConfig(string name, ClothesConfig clothesConf)
    {

        if (pickedClothesConfigs.ContainsKey(name))
        {
            pickedClothesConfigs.Remove(name);
        }
            pickedClothesConfigs.Add(name, clothesConf);
        }

    public void OnBeforeSerialize()
    {
        configNames?.Clear();
        allConfigs?.Clear();

        configNames = pickedClothesConfigs.Keys.ToList();
        allConfigs = pickedClothesConfigs.Values.ToList();
    }

    public void OnAfterDeserialize()
    {
        pickedClothesConfigs = configNames.Zip(allConfigs, (k, v) => new { Key = k, Value = v })
              .ToDictionary(x => x.Key, x => x.Value);

        /* Messenger.Broadcast(GameEvents.CLOTHES_CONFIG_LOADED, this);*/
    }

    public ClothesConfig GetClothesConfig(string key)
    {
        if(pickedClothesConfigs.ContainsKey(key))
        {
            return pickedClothesConfigs[key];
        }
        return null;
    }

    public void SetNickName(string nick)
    {
        nickName = nick;
    }

    public void SetCharGender(Gender sex)
    {
        gender = sex;
    }


/*    public ItemVariant GetActiveVariant(ItemConfig itemConfig)
    {

        foreach (KeyValuePair<ItemConfig, string> pair in pickedClothesConfigs)
        {
            if (pair.Key == itemConfig)
            {
                return ScriptableList<ItemVariant>.instance.GetItemByID(pair.Value);
            }
        }
        return null;
    }

    public bool ItemIsInConfig(ItemConfig item)
    {
        foreach (ItemConfig key in pickedClothesConfigs.Keys)
        {
            if (key == item) return true;
        }
        return false;
    }*/
}

