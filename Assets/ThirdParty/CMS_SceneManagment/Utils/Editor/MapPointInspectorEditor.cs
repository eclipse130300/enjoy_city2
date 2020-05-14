
#if UNITY_EDITOR
using Assets.Scripts.Game.Objects.Scene;
using CMS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Assets.Scripts.Game.Objects.Scene
{
/*    [CustomEditor(typeof(*//*MapPoints*//*))]
    public class MapPointInspectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapPoints targetMapPoints = (MapPoints)target;
            targetMapPoints.prefabForSpawnerPlace = ScriptableGUIUtils.DrawObjectField<PlayerSpawnerPlace>("PrefabForSpawnerPlace", targetMapPoints.prefabForSpawnerPlace);
            targetMapPoints.MaxPlayerCount = Mathf.Clamp(EditorGUILayout.IntField("MaxPlayerCount", targetMapPoints.MaxPlayerCount), 0, 10);
        }

    }*/
}
#endif