using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemClick : MonoBehaviour, IPointerClickHandler
{
    private IItemHandler item;

    private void Awake()
    {
        item = GetComponent<IItemHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!item.IsPreviewing)
        {
            item.ItemPressed();
        }
        else
        {
            item.ItemPicked();
        }
    }
}
