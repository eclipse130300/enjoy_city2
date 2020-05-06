using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour , IPointerClickHandler, IPointerExitHandler
{
    private Image[] allIMG;
    private Image frameIMG;
    
    [SerializeField] private Vector2 sizeToDisplay;
    [SerializeField] private Color activeFrameColor;

    private Transform parent;
    private Color startFrameColor;
    private bool isActive;
    private GameObject item;
    
    private void Awake()
    {
        allIMG = GetComponentsInChildren<Image>();
        parent = GameObject.FindWithTag("bleak_background").transform;
        
        foreach (Image img in allIMG)
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
        if (!isActive)
        {
            item = Instantiate(frameIMG.transform.parent.gameObject, parent.transform.position,
                Quaternion.identity);
            item.transform.SetParent(parent);
            item.GetComponent<RectTransform>().sizeDelta = sizeToDisplay;
            Destroy(item.GetComponent<ItemDisplay>());
            frameIMG.color = activeFrameColor; //todo make field
            isActive = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        frameIMG.color = startFrameColor;
        Destroy(item);
        isActive = false;
    }
}
