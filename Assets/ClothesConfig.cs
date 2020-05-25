using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;
using System;
using System.Linq;

[Serializable]
public class ClothesConfig  : ISerializationCallbackReceiver
{
    public List<ItemConfig> items = new List<ItemConfig>(); 
    public List<string> activeVariantNames = new List<string>();
    public Gender sexForCFG;

    public Dictionary<ItemConfig, string> pickedItemAndVariants = new Dictionary<ItemConfig, string>(); //нужна тольк во время выполнения программы
                                                                                                        //для связи шмотки и айди активного варианта


    public void LoadItemsData()
    {
        pickedItemAndVariants = items.Zip(activeVariantNames, (k, v) => new { Key = k, Value = v })
             .ToDictionary(x => x.Key, x => x.Value);


        Messenger.Broadcast(GameEvents.CLOTHES_CONFIG_LOADED, this);

    }

    public void AdditemToConfig(ItemConfig config, string activeVariant,  Gender sex)
    {
        if (pickedItemAndVariants.IsNullOrEmpty())
        {
            pickedItemAndVariants.Add(config, activeVariant);
        }
        else
        {
            foreach (ItemConfig item in pickedItemAndVariants.Keys.ToArray())
            {
                if (config.bodyPart == item.bodyPart)
                {
                    pickedItemAndVariants.Remove(item);
                }
            }
            pickedItemAndVariants.Add(config, activeVariant);
        }
        sexForCFG = sex;
    }

    public void AdditemToConfig(ItemConfig config, string activeVariant)
    {
        if (pickedItemAndVariants.IsNullOrEmpty())
        {
            pickedItemAndVariants.Add(config, activeVariant);
        }
        else
        {
            foreach (ItemConfig item in pickedItemAndVariants.Keys.ToArray())
            {
                if (config.bodyPart == item.bodyPart)
                {
                    pickedItemAndVariants.Remove(item);
                }
            }
            pickedItemAndVariants.Add(config, activeVariant);
        }
    }

    public ItemVariant GetActiveVariant(ItemConfig itemConfig)
    {

        foreach (KeyValuePair <ItemConfig, string> pair in pickedItemAndVariants)
        {
            if(pair.Key == itemConfig)
            {
                return ScriptableList<ItemVariant>.instance.GetItemByID(pair.Value);
            }
        }
        return null;
    }

    public void OnBeforeSerialize()
    {
        activeVariantNames.Clear();
        items.Clear();

        items = pickedItemAndVariants.Keys.ToList();
        activeVariantNames = pickedItemAndVariants.Values.ToList();
    }

    public void OnAfterDeserialize()
    {
    }

    public bool ItemIsInConfig(ItemConfig item)
    {
        foreach (ItemConfig key in pickedItemAndVariants.Keys)
        {
            if (key == item) return true;
        }
        return false;
    }



    /*    public ClothesConfig()
        {
            foreach(string name in Enum.GetNames(typeof( BODY_PART)))
            {
            }
        }*/
}
