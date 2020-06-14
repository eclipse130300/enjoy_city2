using System.Collections;
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

    public class ItemConfig : BaseScriptableDrowableItem , IDataConfig
    {
        [Draw(DrawAttributeTypes.NotForDraw, "")]
        public Mesh mesh;
        public GameObject itemObject;
        public Color Inventory_frameColor;
        public Sprite Inventory_image;
        public Gender gender;
        public BODY_PART bodyPart;
        public GameMode gameMode;
        public List<ItemVariant> variants = new List<ItemVariant>();
        public bool isDefault;


#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();

            mesh = ScriptableGUIUtils.DrawObjectField("Mesh", mesh);
            itemObject = EditorGUILayout.ObjectField(itemObject, typeof(GameObject), false) as GameObject;
            gender = (Gender)ScriptableGUIUtils.DrawField("GENDER", gender);
            bodyPart = (BODY_PART)ScriptableGUIUtils.DrawField("BODYPART", bodyPart);
            gameMode = (GameMode)ScriptableGUIUtils.DrawField("GameMode", gameMode);
            Inventory_frameColor = ScriptableGUIUtils.DrawField("Inventory_FrameColor", Inventory_frameColor);
            Inventory_image = ScriptableGUIUtils.DrawObjectField("Inventory_icon", Inventory_image);
            gender = (Gender)ScriptableGUIUtils.DrawField("GENDER", gender);
            ScriptableGUIUtils.DrawList("ItemVariants", variants);
            isDefault = ScriptableGUIUtils.DrawField("isDefault?", isDefault);


            
        }
#endif
    }
}

