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

        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.AddListener<RoomItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
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

    public void SetItem(Sprite inventoryIMG, Color frameCol)
    {
        inventoryImage.sprite = inventoryIMG;
        startFrameColor = frameCol;
        frameIMG.color = frameCol;

    }

    public void ItemPicked()
    {

        isPreviewing = false;
        frameIMG.color = startFrameColor;
        Messenger.Broadcast(GameEvents.ITEM_PICKED);
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
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
        Messenger.RemoveListener<RoomItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, OnItemBought);
    }
}
