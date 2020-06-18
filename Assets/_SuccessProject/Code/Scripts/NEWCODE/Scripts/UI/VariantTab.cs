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
    public GameObject activeTick;

    public VariantGroup group;
    public ItemVariant variant;

    private void Awake()
    {
        Messenger.AddListener<RoomItemConfig,ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnRoomItemBought);
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);

        Messenger.AddListener<ItemConfig>(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.AddListener<RoomItemConfig>(GameEvents.ROOM_ITEM_PICKED, OnRoomItemPicked);
    }

    private void OnRoomItemPicked(RoomItemConfig cfg)
    {
        var activeVar = SaveManager.Instance.LoadRoomSet().GetActiveVariant(cfg);

        bool isActive = activeVar == variant ? true : false;
        activeTick.SetActive(isActive);
    }

    private void OnItemPicked(ItemConfig cfg)
    {
        var activeVar = SaveManager.Instance.LoadClothesSet(PreviewManager.GetCurrentKey()).GetActiveVariant(cfg);

        bool isActive = activeVar == variant ? true : false;
        activeTick.SetActive(isActive);
    }

    private void OnRoomItemBought(RoomItemConfig cfg, ItemVariant var)
    {
        if (variant == var)
        {
            lockIMG?.gameObject.SetActive(false);
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
        Messenger.RemoveListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnRoomItemBought);

        Messenger.RemoveListener<ItemConfig>(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener<RoomItemConfig>(GameEvents.ROOM_ITEM_PICKED, OnRoomItemPicked);
    }
}

