using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConfigInventory : MonoBehaviour , IInventoryDisplayer<ItemConfig>
{

    public List<ItemConfig> inventory { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void InitializeItem()
    {
        throw new System.NotImplementedException();
    }

    public void InstantiateItem()
    {
        throw new System.NotImplementedException();
    }

    public void SortItems()
    {
        throw new System.NotImplementedException();
    }
}
