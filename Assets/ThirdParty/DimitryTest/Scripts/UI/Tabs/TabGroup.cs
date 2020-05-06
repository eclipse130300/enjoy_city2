using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<Tab> tabs;

    public bool withTEXT;

    public Sprite inactivebackgroundIMG;
    public Sprite activebackgroundIMG;
    
    public Color activeColor;
    public Color normalColor;

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
        if (withTEXT)
        {
            tab.tmPro.color = activeColor;
        }
    }

    void ResetTabs()
    {
        foreach (Tab tab in tabs)
        {
            tab.background.sprite = inactivebackgroundIMG;
            tab.icon.color = normalColor;
            tab.GetComponent<RectTransform>().sizeDelta = inactiveSize;
            if (withTEXT)
            {
                tab.tmPro.color = normalColor;
            }
        }
    }
}
