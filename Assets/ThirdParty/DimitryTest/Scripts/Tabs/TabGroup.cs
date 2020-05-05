﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<Tab> tabs;

    public bool withTEXT;

    public Sprite inactivebackgroundIMG;
    public Sprite activebackgroundIMG;
    public Color activeColor;

    private Color standardColor = Color.white;

    public Vector2 activeSize;
    public Vector2 inactiveSize;



    private void Start()
    {
        ResetTabs();
    }

    public void Subscribe(Tab tabButton)
    {
        if(tabs == null)
        {
            tabs = new List<Tab>();
        }

        tabs.Add(tabButton);
    }

    public void OnTabSelected(Tab tab)
    {
        ResetTabs();
        tab.background.sprite = activebackgroundIMG;
        tab.icon.color = activeColor;
        tab.GetComponent<RectTransform>().sizeDelta = activeSize;
        if(withTEXT)
        {
            tab.GetComponentInChildren<TextMeshPro>().color = activeColor;
        }
    }

    void ResetTabs()
    {
        foreach (Tab tab in tabs)
        {
            tab.background.sprite = inactivebackgroundIMG;
            tab.icon.color = standardColor;
            tab.GetComponent<RectTransform>().sizeDelta = inactiveSize;
/*            if (withTEXT)
            {
                tab.GetComponentInChildren<TextMeshPro>().color = standardColor;
            }*/
        }
    }
}
