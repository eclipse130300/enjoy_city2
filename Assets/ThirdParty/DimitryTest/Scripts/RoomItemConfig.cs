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
        public Vector3 position;



#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();

            //mapIcon = ScriptableGUIUtils.DrawObjectField("MapIcon", mapIcon);

            prefab = ScriptableGUIUtils.DrawObjectField("PREFAB", prefab);
            color = ScriptableGUIUtils.DrawField("Color", color);
            position = ScriptableGUIUtils.DrawField("PositionVector3", position);

            //experienceToNextLevel = ScriptableGUIUtils.DrawField("ExperienceToNextLevel", experienceToNextLevel);

        }
#endif
    }
}
