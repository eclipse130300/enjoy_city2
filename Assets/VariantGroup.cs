using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariantGroup : MonoBehaviour
{
    List<VariantTab> variants = new List<VariantTab>();

    public void OnVariantSelected(VariantTab tab)
    {
        ResetVariantsFrameAlpha();
        tab.activeIMG.gameObject.SetActive(true);
    }

    public void ResetVariantsFrameAlpha()
    {
        foreach (VariantTab tab in variants)
        {
            if(tab.activeIMG != null)  tab.activeIMG.gameObject.SetActive(false);
        }
    }

/*    public void SetTabFrameAlpha(float alpha, VariantTab tab)
    {
        if (tab.activeIMG != null)
        {
            Color c = tab.activeIMG.color;
            c.a = alpha;
            tab.activeIMG.color = c;
        }
    }*/


    public void Subscribe(VariantTab tab)
    {
        if (variants == null) variants = new List<VariantTab>();
        variants.Add(tab);
    }
}
