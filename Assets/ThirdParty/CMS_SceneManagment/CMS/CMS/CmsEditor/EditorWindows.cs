﻿using UnityEditor;
using CMS.Config;
using CMS.Configs;
using CMS.Configs.Audio.Settings;


#if UNITY_EDITOR
namespace CMS.Editor
{
    public class EditorWindows
    {

    }

    public class MapEditorWindow : BaseListEditor<MapConfig>
    {
        [MenuItem("Configs/Maps %#n")]
        public static void InitResourcesEditorWindow()
        {
            EditorWindow.GetWindow(typeof(MapEditorWindow));
        }
    }

    public class LevelEditorWindow : BaseListEditor<LevelConfig>
    {
        [MenuItem("Configs/PlayerLevel %#n")]
        public static void InitResourcesEditorWindow()
        {
            EditorWindow.GetWindow(typeof(LevelEditorWindow));
        }
    }

    public class RoomItemEditorWindow : BaseListEditor<RoomItemConfig>
    {
        [MenuItem("Configs/RoomItemCFG %#n")]
        public static void InitResourcesEditorWindow()
        {
            EditorWindow.GetWindow(typeof(RoomItemEditorWindow));
        }
    }

    public class ItemEditorWindow : BaseListEditor<ItemConfig>
    {
        [MenuItem("Configs/ItemConfig %#n")]
        public static void InitResourcesEditorWindow()
        {
            EditorWindow.GetWindow(typeof(ItemEditorWindow));
        }
    }

    public class ItemVariantWindow : BaseListEditor<ItemVariant>
    {
        [MenuItem("Configs/ItemVariant %#n")]
        public static void InitResourcesEditorWindow()
        {
            EditorWindow.GetWindow(typeof(ItemVariantWindow));
        }
    }

}
#endif
