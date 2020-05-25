using UnityEngine;
using UnityEngine.EventSystems;

public class GameModeInventoryChanger : MonoBehaviour, IPointerClickHandler
{
    public GameMode gamemode;

    public void OnPointerClick(PointerEventData eventData)
    {
        Messenger.Broadcast(GameEvents.INVENTORY_GAME_MODE_CHANGED, gamemode);
        Messenger.Broadcast(GameEvents.ITEM_OPERATION_ABORT);
    }

}
