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
    public class RoomItemConfig : BaseScriptableDrowableItem
    {
        [Draw(DrawAttributeTypes.NotForDraw, "")]
        public GameObject prefab;
        public Color color;



#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
           
            prefab = ScriptableGUIUtils.DrawObjectField("PREFAB", prefab);
            color = ScriptableGUIUtils.DrawField("Color", color);
        }
#endif
    }
}
