using CMS.Config;
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
    private bool isPreviewing;

    public Image lockIcon;
    public GameObject activeItemTick;

    private ShopManager shopManager;

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
        }

        shopManager = ShopManager.Instance;
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.AddListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnItemBought);
        Messenger.AddListener<RoomItemDisplay>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);
    }

    private void OnItemPicked(RoomItemDisplay itemDisplay)
    {
        if (this == itemDisplay && shopManager.CheckIfItemIsBought(itemDisplay.itemConfig))
        {
            activeItemTick.SetActive(true);
        }
        else
        {
            activeItemTick.SetActive(false);
        }
    }

    private void OnItemBought(RoomItemConfig cfg, ItemVariant var) // TODO var is unnecessary
    {
        if (itemConfig == cfg)
        {
            if (lockIcon != null)
            {
                lockIcon.gameObject?.SetActive(false);
            }
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

    public void ItemPicked()
    {
        isPreviewing = false;
        frameIMG.color = startFrameColor;
        Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);

        if (shopManager.CheckIfItemIsBought(this.itemConfig))
        {
            Messenger.Broadcast(GameEvents.ROOM_ITEM_PICKED, this);
        }
    }

    public void ItemPressed()
    {
        isPreviewing = true;
        Messenger.Broadcast(GameEvents.ITEM_PRESSED, gameObject);
        frameIMG.color = previewFrameColor;
    }

    public void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.RemoveListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, OnItemBought);
        Messenger.RemoveListener<RoomItemDisplay>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);
    }
}
