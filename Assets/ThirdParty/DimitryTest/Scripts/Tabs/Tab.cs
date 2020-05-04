using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] private Image inactivebackgroundIMG;
    [SerializeField] private Image activebackgroundIMG;
    [SerializeField] private Image activeIconIMG;
    [SerializeField] private Image inactiveIconIMG;

    public void OnPointerClick(PointerEventData eventData)
    {
        ActivateTab();
    }

    public void ActivateTab()
    {

    }

    public void DeactivateTab()
    {

    }


}
