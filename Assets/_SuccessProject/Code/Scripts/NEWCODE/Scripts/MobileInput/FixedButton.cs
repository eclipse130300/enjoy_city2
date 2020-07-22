using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    [HideInInspector]
    public bool Pressed;
    public bool Clicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(Click());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }

    IEnumerator Click()
    {
        Clicked = true;
        yield return new WaitForEndOfFrame();
        Clicked = false;
    }
}