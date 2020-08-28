using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IItemHandler
{

    [SerializeField] private Color previewFrameColor;

    public ItemConfig itemConfig;
    public Image inventoryImage;

    private Image frameIMG;
    private Color startFrameColor;

    public Image lockIcon;
    public GameObject activeItemTick;

    private ShopManager shopManager;
    public bool isClicked;

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
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
        Messenger.AddListener<ItemConfig>(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, OnItemDone);
    }

    private void OnItemDone()
    {
        frameIMG.color = startFrameColor;
        isClicked = false;
    }

    private void OnItemPicked(ItemConfig itemConfig)
    {
        if (this.itemConfig == itemConfig && shopManager.CheckIfItemIsBought(itemConfig))
        {

            activeItemTick.SetActive(true);
        }
        else
        {
            activeItemTick.SetActive(false);
        }

/*        frameIMG.color = startFrameColor;
        isPreviewing = false;*/
    }

    private void OnItemBought(ItemConfig cfg, ItemVariant var) // TODO var is unnecessary
    {
        if (itemConfig == cfg)
        {
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(false);
                activeItemTick.SetActive(true);
/*                frameIMG.color = startFrameColor;*/
            }
        }
        else
        {
            activeItemTick.SetActive(false);
        }
    }

    private void ClearIfOtherItem(GameObject item)
    {
        if (gameObject == item) return;
        frameIMG.color = startFrameColor;
        isClicked = false;
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
        if (!isClicked)
        {
            isClicked = true;
            Messenger.Broadcast(GameEvents.ITEM_PRESSED, gameObject);
            frameIMG.color = previewFrameColor;
        }
    }

    public void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.RemoveListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
        Messenger.RemoveListener<ItemConfig>(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, OnItemDone);
    }
}