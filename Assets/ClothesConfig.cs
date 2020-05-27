using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;
using System;
using System.Linq;

[Serializable]
public class ClothesConfig : ISerializationCallbackReceiver
{
    public List<string> pickedItemsAndVariants = new List<string>();

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
        if (pickedItemsAndVariants != null)
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
                /*return item.variants[0];*/
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
