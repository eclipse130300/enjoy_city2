using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;
using System.Linq;

[System.Serializable]
public class ChangableDataConfig : ISerializationCallbackReceiver, IDataConfig
{

    public List<ClothesConfig> allClothesConfigs = new List<ClothesConfig>();
    public List<string> clothesConfigNames = new List<string>();

    public RoomConfig roomConfig = new RoomConfig();


    string nickName;
    public string currentBodyConfigId ;

    public Dictionary<string, ClothesConfig> pickedClothesConfigs = new Dictionary<string, ClothesConfig>(); //нужна тольк во время выполнения программы
                                                                                                             //для связи шмотки и айди активного варианта

    public void AddClothesConfig(string name, ClothesConfig clothesConf)
    {

        if (pickedClothesConfigs.ContainsKey(name))
        {
            pickedClothesConfigs.Remove(name);
        }
            pickedClothesConfigs.Add(name, clothesConf);
        }

    public void OnBeforeSerialize()
    {
        clothesConfigNames?.Clear();
        allClothesConfigs?.Clear();

        clothesConfigNames = pickedClothesConfigs.Keys.ToList();
        allClothesConfigs = pickedClothesConfigs.Values.ToList();
    }

    public void OnAfterDeserialize()
    {
        pickedClothesConfigs = clothesConfigNames.Zip(allClothesConfigs, (k, v) => new { Key = k, Value = v })
              .ToDictionary(x => x.Key, x => x.Value);

        /* Messenger.Broadcast(GameEvents.CLOTHES_CONFIG_LOADED, this);*/
    }

    public ClothesConfig GetClothesConfig(string key)
    {
        if(pickedClothesConfigs.ContainsKey(key))
        {
            return pickedClothesConfigs[key];
        }
        return new ClothesConfig();
    }



    public void SetNickName(string nick)
    {
        nickName = nick;
    }

    public string GetNickName()
    {
        return nickName;
    }

/*    public void SetCharGender(Gender sex)
    {
        gender = sex;
    }*/


}

