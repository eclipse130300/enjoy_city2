using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour , IPointerClickHandler, IPointerExitHandler
{
    private Image[] allIMG;
    private Image frameIMG;
    public Transform parent;

    [SerializeField] GameObject itemDescriptionPosition;

    private void Awake()
    {
        allIMG = GetComponentsInChildren<Image>();
        
        foreach (Image img in allIMG)
        {
            if(img.gameObject.CompareTag("frame"))
            {
                frameIMG = img;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        frameIMG.color = Color.yellow;
        var item = Instantiate(frameIMG.gameObject.transform.parent.gameObject , itemDescriptionPosition.transform.position , Quaternion.identity);
        item.transform.SetParent(parent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        frameIMG.color = Color.white;
    }
}
