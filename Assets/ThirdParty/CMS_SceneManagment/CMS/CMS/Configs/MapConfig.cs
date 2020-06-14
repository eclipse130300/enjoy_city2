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
    public class MapConfig : BaseScriptableDrowableItem
    {
        [Draw(DrawAttributeTypes.ObjectSellectionField, "mapIcon")]
        public Sprite mapIcon;
        
        [Draw(DrawAttributeTypes.NotForDraw, "")]

        public string SceneName = "";
        public string InterfaceSceneName = "";
        public string ManagersSceneName = "";

        public AudioClip backGruondMusic;

        public GameMode gameMode;
        /*
                public GameMod ForGameMod;*/
#if UNITY_EDITOR
        public SceneAsset Scene;
        public SceneAsset InterfaceScene;
        public SceneAsset ManagersScene;



        public override void Draw()
        {
            base.Draw();
            gameMode = (GameMode)ScriptableGUIUtils.DrawField("gameMode", gameMode);
            mapIcon = ScriptableGUIUtils.DrawObjectField("MapIcon",mapIcon);
     
            Scene = ScriptableGUIUtils.DrawObjectField("Game Scene", Scene);
            InterfaceScene = ScriptableGUIUtils.DrawObjectField("Interface Scene", InterfaceScene);
            ManagersScene = ScriptableGUIUtils.DrawObjectField("Managers Scene", ManagersScene);


            backGruondMusic = ScriptableGUIUtils.DrawObjectField("BackGroundMusic", backGruondMusic);

            if (Scene)
                SceneName = Scene.name;
            else
                SceneName = "";
            if (InterfaceScene)
                InterfaceSceneName = InterfaceScene.name;
            else
                InterfaceSceneName = "";
            if (ManagersScene)
                ManagersSceneName = ManagersScene?.name;
            else
                ManagersSceneName = "";
        }


#endif
    }
}
