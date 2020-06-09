using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour , IItemHandler
{
    [SerializeField] private Color previewFrameColor;

    public ItemConfig itemConfig;
    public Image inventoryImage;

    private Image frameIMG;
    private Color startFrameColor;
    private bool isPreviewing;

    public Image lockIcon;
    public GameObject activeItemTick;

    public bool IsPreviewing
    {
        get => isPreviewing;
        set => isPreviewing = value;
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

            else if (img.gameObject.CompareTag("activeItemTick"))
            {
                activeItemTick = img.gameObject;
            }
        }

        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
        Messenger.AddListener<ItemDisplay>(GameEvents.ITEM_PICKED, OnItemPicked);
    }

    private void OnItemPicked(ItemDisplay itemDisplay)
    {
        if (this == itemDisplay)
        {
            activeItemTick.SetActive(true);
        }
        else
        {
            activeItemTick.SetActive(false);
        }
    }

    private void OnItemBought(ItemConfig cfg, ItemVariant var) // TODO var is unnecessary
    {
        if (itemConfig == cfg)
        {
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(false);
                activeItemTick.SetActive(true);
                frameIMG.color = startFrameColor;
                isPreviewing = false;
            }
        }
        else
        {
            activeItemTick.SetActive(false);
        }
    }

    private void OnItemPressed(GameObject item)
    {
        if (this == item.GetComponent<ItemDisplay>()) return;
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

    public void ItemPicked()
    {
        isPreviewing = false;
        frameIMG.color = startFrameColor;
        Messenger.Broadcast(GameEvents.ITEM_PICKED, this);
        Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);
    }

    public void ItemPressed()
    {
        isPreviewing = true;
        Messenger.Broadcast(GameEvents.ITEM_PRESSED, gameObject);
        frameIMG.color = previewFrameColor;
    }

    public void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.RemoveListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
        Messenger.RemoveListener<ItemDisplay>(GameEvents.ITEM_PICKED, OnItemPicked);
    }
}

