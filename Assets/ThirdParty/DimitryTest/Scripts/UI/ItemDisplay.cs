using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Color previewFrameColor;

    public ItemConfig itemConfig;
    public Image inventoryImage;

    private Image frameIMG;
    private Color startFrameColor;
    private bool isPreviewing;



    private void Awake()
    {
        Image[] allImages = GetComponentsInChildren<Image>();  

        foreach (Image img in allImages)
        {
            if (img.gameObject.CompareTag("frame"))
            {
                frameIMG = img;
            }
        }

        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
    }

    private void ClearIfOtherItem(GameObject item)
    {
        if (this == item.GetComponent<ItemDisplay>()) return;
        frameIMG.color = startFrameColor;
        isPreviewing = false;
    }

    public void SetItem(Sprite inventoryIMG, Color frameCol)
    {
        inventoryImage.sprite = inventoryIMG;
        startFrameColor = frameCol;
        frameIMG.color = frameCol;
    }

    private void ItemPicked()
    {

        isPreviewing = false;
        frameIMG.color = startFrameColor;
        Messenger.Broadcast(GameEvents.ITEM_PICKED);
        Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);
    }

    private void ItemPressed()
    {
        isPreviewing = true;
        Messenger.Broadcast(GameEvents.ITEM_PRESSED, gameObject);
        frameIMG.color = previewFrameColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPreviewing)
        {
            ItemPressed();
        }
        else
        {
            ItemPicked();
        }
    }

    public void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, ClearIfOtherItem);
    }

}

