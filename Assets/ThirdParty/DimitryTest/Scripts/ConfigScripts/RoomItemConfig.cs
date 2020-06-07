using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using Assets.Scripts.Game.Objects.Scene;
using CMS.Editor;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace CMS.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class RoomItemConfig : BaseScriptableDrowableItem
    {
        [Draw(DrawAttributeTypes.NotForDraw, "")]
        public GameObject prefab;
        public Color color;
        public Sprite Inventory_image;
        public Color Inventory_frameColor;
        public FURNITURE furnitureType;
        public Material material;
        public List<ItemVariant> variants = new List<ItemVariant>();

#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
           
            prefab = ScriptableGUIUtils.DrawObjectField("PREFAB", prefab);
            color = ScriptableGUIUtils.DrawField("Color", color);
            material = ScriptableGUIUtils.DrawObjectField("material", material);

            furnitureType = (FURNITURE)ScriptableGUIUtils.DrawField("furnitureType", furnitureType);
            Inventory_frameColor = ScriptableGUIUtils.DrawField("Inventory_FrameColor", Inventory_frameColor);
            Inventory_image = ScriptableGUIUtils.DrawObjectField("Inventory_icon", Inventory_image);
            ScriptableGUIUtils.DrawList("ItemVariants", variants);



        }
#endif
    }
}
