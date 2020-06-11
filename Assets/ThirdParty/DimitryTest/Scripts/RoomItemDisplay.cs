using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemDisplay : MonoBehaviour, IItemHandler
{

    [SerializeField] private Color previewFrameColor;

    public RoomItemConfig itemConfig;
    public Image inventoryImage;

    private Image frameIMG;
    private Color startFrameColor;

    public Image lockIcon;
    public GameObject activeItemTick;

    private ShopManager shopManager;
    bool isPreviewing;

    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    private void Awake()
    {
        Image[] allImages = GetComponentsInChildren<Image>();

        foreach (Image img in allImages)
        {
            if (img.gameObject.CompareTag("frame"))
            {
                frameIMG = img;
            }

            else if (img.gameObject.CompareTag("lock"))
            {
                lockIcon = img;
            }
        }

        shopManager = ShopManager.Instance;
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.AddListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnItemBought);
        Messenger.AddListener<RoomItemConfig>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);
        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, OnItemDone);
    }

    private void OnItemDone()
    {
        frameIMG.color = startFrameColor;
        isPreviewing = false;
    }

    private void OnItemPicked(RoomItemConfig itemConfig)
    {
        if (this.itemConfig == itemConfig && shopManager.CheckIfItemIsBought(itemConfig))
        {

            activeItemTick.SetActive(true);
        }
        else
        {
            activeItemTick.SetActive(false);
        }

        frameIMG.color = startFrameColor;
        isPreviewing = false;
    }

        private void OnItemBought(RoomItemConfig cfg, ItemVariant var) // TODO var is unnecessary
    {
        if (itemConfig == cfg)
        {
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(false);
                activeItemTick.SetActive(true);
                frameIMG.color = startFrameColor;
            }
        }
        else
        {
            activeItemTick.SetActive(false);
        }
    }

    private void ClearIfOtherItem(GameObject item)
    {
        if (this == item.GetComponent<RoomItemDisplay>()) return;
        frameIMG.color = startFrameColor;
        isPreviewing = false;
    }

    public void SetItem(Sprite inventoryIMG, Color frameCol, bool isActiveItem)
    {
        inventoryImage.sprite = inventoryIMG;
        startFrameColor = frameCol;
        frameIMG.color = frameCol;
        activeItemTick.SetActive(isActiveItem);
    }

    public void ItemPressed()
    {
        if (!isPreviewing)
        {
            isPreviewing = true;
            Messenger.Broadcast(GameEvents.ITEM_PRESSED, gameObject);
            frameIMG.color = previewFrameColor;
        }
    }

    public void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.RemoveListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnItemBought);
        Messenger.RemoveListener<RoomItemConfig>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, OnItemDone);
    }
}
