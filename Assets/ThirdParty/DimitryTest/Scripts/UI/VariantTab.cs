using CMS.Config;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VariantTab : MonoBehaviour, IPointerClickHandler
{
    public Image activeIMG;
    public Image tabBackground;
    public Image lockIMG;

    public VariantGroup group;
    public ItemVariant variant;

    private void Awake()
    {
        Messenger.AddListener<RoomItemConfig,ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnRoomItemBought);
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
    }

    private void OnRoomItemBought(RoomItemConfig cfg, ItemVariant var)
    {
        if (variant == var)
        {
            lockIMG.gameObject.SetActive(false);
        }
    }

    private void OnItemBought(ItemConfig cfg, ItemVariant var)
    {
        if (variant == var)
        {
            lockIMG.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        group.OnVariantSelected(this);
        Messenger.Broadcast(GameEvents.ITEM_VARIANT_CHANGED, variant); //texture as well
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
        Messenger.AddListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnRoomItemBought);
    }
}

