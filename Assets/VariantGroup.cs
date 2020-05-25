using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantGroup : MonoBehaviour
{
    List<VariantTab> variants = new List<VariantTab>();

    public void OnVariantSelected(VariantTab tab)
    {
        ResetVariantsFrameAlpha();
        SetTabFrameAlpha(1, tab);
    }

    public void ResetVariantsFrameAlpha()
    {
        foreach (VariantTab tab in variants)
        {
            SetTabFrameAlpha(0, tab);
        }
    }

    public void SetTabFrameAlpha(float alpha, VariantTab tab)
    {
        if (tab.activeIMG != null)
        {
            Color c = tab.activeIMG.color;
            c.a = alpha;
            tab.activeIMG.color = c;
        }
    }

    public void Subscribe(VariantTab tab)
    {
        if (variants == null) variants = new List<VariantTab>();
        variants.Add(tab);
    }
}
