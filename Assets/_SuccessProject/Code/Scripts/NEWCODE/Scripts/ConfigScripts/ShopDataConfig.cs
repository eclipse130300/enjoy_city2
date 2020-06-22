using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopDataConfig : IDataConfig
{
    public List<string> boughtRoomItems = new List<string>(); //contains item_id+itemVar(both bougt)
    public List<string> bought3DModelItems = new List<string>(); //last thing to do

    public void AddItemToBoughtList(ItemConfig cfg, ItemVariant variant)
    {
        var itemID = cfg.ConfigId;
        var variantID = variant.ConfigId;

        var resultString = string.Concat(itemID, "+", variantID);
        bought3DModelItems.Add(resultString);
    }

    public bool IsModelItemInBoughtList(ItemConfig item)
    {
        foreach (string dirtyPair in bought3DModelItems)
        {
            string[] strs = dirtyPair.Split('+');
            if (strs.Contains(item.ConfigId))
            {
                return true;
            }
        }
        return false;
    }

    public bool VariantIsBought(ItemConfig item, ItemVariant variant)
    {
        foreach (string dirtyPair in bought3DModelItems)
        {
            string[] strs = dirtyPair.Split('+');
            if (strs.Contains(item.ConfigId))
            {
                if(strs.Contains(variant.ConfigId))
                {
                    return true;
                }
            }
        }
        return false;
    }
}

