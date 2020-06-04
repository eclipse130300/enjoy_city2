using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public interface IInventoryDisplayer <T> where T: BaseScriptableDrowableItem
{
    List<T> inventory { get; set; }

    void SortItems();
    void InstantiateItem();
    void InitializeItem();


}
