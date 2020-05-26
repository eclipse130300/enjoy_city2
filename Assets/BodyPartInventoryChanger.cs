using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPartInventoryChanger : MonoBehaviour, IPointerClickHandler
{
    public BODY_PART bodypart;

    public void OnPointerClick(PointerEventData eventData)
    {
        Messenger.Broadcast(GameEvents.INVENTORY_BODY_PART_CHANGED, bodypart);
        Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);
    }

}
