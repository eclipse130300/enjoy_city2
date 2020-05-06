using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActivator : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    private Color frameIMGcolor;
    private void Awake()
    {
        frameIMGcolor = GetComponent<Image>().color;
    }
    // Start is called before the first frame update
    void Start()
    {
        frameIMGcolor.a = 0;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        frameIMGcolor.a = 1;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        frameIMGcolor.a = 0;
    }
}
