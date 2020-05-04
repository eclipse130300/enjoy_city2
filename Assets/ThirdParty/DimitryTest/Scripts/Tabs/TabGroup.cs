using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<Tab> tabs;

   public void Subscribe(Tab tabButton)
    {
        if(tabs == null)
        {
            tabs = new List<Tab>();
        }

        tabs.Add(tabButton);
    }

    public void OnTabSelected()
    {

    }
}
