﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using Assets.Scripts.Game.Objects.Scene;
using CMS.Editor;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace CMS.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]

    public class ItemVariant : IGUIDrawable
    {
        [Draw(DrawAttributeTypes.NotForDraw, "")]
        public string ConfigId;
        public Color color;
        public Texture texture;
        public int cost;
        public CurrencyType currencyType;

#if UNITY_EDITOR
        public  void Draw()
        {
            color = ScriptableGUIUtils.DrawField("Color", color);
            currencyType = (CurrencyType)ScriptableGUIUtils.DrawField("Currency", currencyType);
            cost = ScriptableGUIUtils.DrawField("Cost", cost);

        }
#endif
    }
}
