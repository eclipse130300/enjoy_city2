using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActivator : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    private Image frameIMGcolor;
    private void Awake()
    {
        frameIMGcolor = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetFrameAlpha(0f);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetFrameAlpha(1);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        SetFrameAlpha(0);
    }

    void SetFrameAlpha(float alpha)
    {
        Color c = frameIMGcolor.color;
        c.a = alpha;
        frameIMGcolor.color = c;
    }
}
