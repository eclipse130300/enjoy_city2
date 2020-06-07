using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemHandler
{
    bool IsPreviewing
    {
        get;
        set;
    }

 void ItemPressed();
 void  ItemPicked();
}
