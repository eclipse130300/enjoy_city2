using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private Color activeFrameColor;
    public ItemConfig itemConfig;

    private Image frameIMG;
    private Color startFrameColor;
    private bool isActive;

    
    private void Awake()
    {
        Image[] allImages = GetComponentsInChildren<Image>();
        
        foreach (Image img in allImages)
        {
            if(img.gameObject.CompareTag("frame"))
            {
                frameIMG = img;
                startFrameColor = frameIMG.color;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemPressed();
    }

    private void ItemPressed()
    {
        if (!isActive)
        {
            isActive = true;
            Messenger.Broadcast(GameEvents.ITEM_PRESSED, gameObject);
            Messenger.Broadcast(GameEvents.PUT_ON_ITEM, itemConfig);
            frameIMG.color = activeFrameColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Messenger.Broadcast(GameEvents.ITEM_UNPRESSED);
        frameIMG.color = startFrameColor;
        isActive = false;
    }
}
