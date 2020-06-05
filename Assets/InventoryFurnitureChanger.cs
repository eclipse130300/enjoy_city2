using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryFurnitureChanger : MonoBehaviour , IPointerClickHandler
{
    public FURNITURE furniture;

    public void OnPointerClick(PointerEventData eventData)
    {
        Messenger.Broadcast(GameEvents.FURNITURE_CHANGED, furniture);
        Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);
    }
}
