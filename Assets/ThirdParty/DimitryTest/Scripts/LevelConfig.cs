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
    public class LevelConfig : BaseScriptableDrowableItem
    {
        [Draw(DrawAttributeTypes.NotForDraw, "")]
        public int level = 0;
        public float experience = 0f;
        public float experienceToNextLevel = 0f;


#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();

            //mapIcon = ScriptableGUIUtils.DrawObjectField("MapIcon", mapIcon);

            level = ScriptableGUIUtils.DrawField("Level", level);
            experience = ScriptableGUIUtils.DrawField("Experience", experience);
            experienceToNextLevel = ScriptableGUIUtils.DrawField("ExperienceToNextLevel", experienceToNextLevel);

        }
#endif
    }
}


