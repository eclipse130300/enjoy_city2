using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour , IPointerClickHandler
{
    public TabGroup tabGroup;

    public Image icon;
    [HideInInspector] public Image background;

    void Start()
    {
        tabGroup.Subscribe(this);
        background = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
        Debug.Log("CLICKED!");
    }
}
