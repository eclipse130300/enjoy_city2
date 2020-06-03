using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using Assets.Scripts.Game.Objects.Scene;
using CMS.Editor;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
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
        public string description;
        public CurrencyType currencyType;

#if UNITY_EDITOR
        public  void Draw()
        {
            ConfigId = ScriptableGUIUtils.DrawField("ConfigID", ConfigId);
            color = ScriptableGUIUtils.DrawField("Color", color);
            currencyType = (CurrencyType)ScriptableGUIUtils.DrawField("Currency", currencyType);
            cost = ScriptableGUIUtils.DrawField("Cost", cost);
            description = ScriptableGUIUtils.DrawField("Description", description);
        }
#endif
    }
}
