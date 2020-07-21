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

    public class BodyConfig : BaseScriptableDrowableItem
    {
        [Draw(DrawAttributeTypes.NotForDraw, "")]

        public GameObject game_body_prefab;
        public GameObject preview_body_prefab;
        public Color body_color;
        public Gender gender;
        public RuntimeAnimatorController controller;
        public Avatar avatar;
        public bool isDefault;

#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();

            game_body_prefab = ScriptableGUIUtils.DrawObjectField("game_body_prefab", game_body_prefab);
            preview_body_prefab = ScriptableGUIUtils.DrawObjectField("preview_body_prefab", preview_body_prefab);

            gender = (Gender)ScriptableGUIUtils.DrawField("GENDER", gender);
            body_color = ScriptableGUIUtils.DrawField("body_color", body_color);

            controller = ScriptableGUIUtils.DrawObjectField("animator_controller", controller);
            avatar = ScriptableGUIUtils.DrawObjectField("avatar", avatar);
            isDefault = ScriptableGUIUtils.DrawField("isDefault?", isDefault);
        }
#endif
    }
}

public enum Gender
{
    MALE,
    FEMALE
}