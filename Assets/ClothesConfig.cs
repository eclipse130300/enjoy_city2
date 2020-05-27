using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;
using System;
using System.Linq;

[Serializable]
public class ClothesConfig : ISerializationCallbackReceiver
{
    /*    public List<ItemConfig> items = new List<ItemConfig>(); 
        public List<string> activeVariantNames = new List<string>();

        public Dictionary<ItemConfig, string> pickedItemAndVariants = new Dictionary<ItemConfig, string>(); *///нужна тольк во время выполнения программы
                                                                                                              //для связи шмотки и айди активного варианта
    public List<string> pickedItemsAndVariants = new List<string>();

    /*    public void LoadItemsData()
        {
            pickedItemAndVariants = items.Zip(activeVariantNames, (k, v) => new { Key = k, Value = v })
                 .ToDictionary(x => x.Key, x => x.Value);


            Messenger.Broadcast(GameEvents.CLOTHES_CONFIG_LOADED, this); //TODO NOT NEEDED?

        }*/

    /*    public void AdditemToConfig(ItemConfig config, string activeVariant)
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
        }*/

    public void AddItemToConfig(ItemConfig item, ItemVariant variant)
    {

        string itemID = item.ConfigId;
        string variantID = variant.ConfigId;

        foreach(string pair in pickedItemsAndVariants.ToList())
        {
            string[] strs = pair.Split('+');
            var it = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);
            Debug.Log(it);
            if(it.bodyPart == item.bodyPart)
            {
                pickedItemsAndVariants.Remove(pair);
            }
        }

        pickedItemsAndVariants.Add(string.Concat(itemID, "+", variantID));
        /*pickedItemsAndVariants.Add(itemID + "_" + variantID);*/
    }

    public void AddItemToConfig(ItemConfig item)
    {
        ItemVariant var = new ItemVariant();

        string itemID = item.ConfigId;
        string variantID = var.ConfigId;

        pickedItemsAndVariants.Add(itemID + "+" + variantID);
    }

    public ItemVariant GetActiveVariant(ItemConfig item)
    {
        foreach (string dirtyPair in pickedItemsAndVariants)
        {
            string[] strs = dirtyPair.Split('+');
            if (strs.Contains(item.ConfigId))
            {
                foreach (ItemVariant var in item.variants)
                {
                    if (strs[1] == var.ConfigId)
                    {
                        return var;
                    }
                }
            }
        }
        return null;
    }

    /*    public ItemVariant GetActiveVariant(ItemConfig itemConfig)
        {

            foreach (KeyValuePair <ItemConfig, string> pair in pickedItemAndVariants)
            {
                if(pair.Key == itemConfig)
                {
                    return ScriptableList<ItemVariant>.instance.GetItemByID(pair.Value);
                }
            }
            return null;
        }*/

    public void OnBeforeSerialize()
    {
        /*        activeVariantNames.Clear();
                items.Clear();

                items = pickedItemAndVariants.Keys.ToList();
                activeVariantNames = pickedItemAndVariants.Values.ToList();*/
    }

    public void OnAfterDeserialize()
    {
    }

    public bool ItemIsInConfig(ItemConfig item)
    {
        foreach (string dirtyPair in pickedItemsAndVariants)
        {
            string[] strs = dirtyPair.Split('+');
            if (strs.Contains(item.ConfigId))
            {
                return true;
            }
        }
            return false;
    }
}
