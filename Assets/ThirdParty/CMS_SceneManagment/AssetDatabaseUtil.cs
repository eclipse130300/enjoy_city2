using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Utils
{
    public static class AssetDatabaseUtil
    {

        public const string REMOVE_FROM_PATH = "Assets/Resources/";

        public static string GetResourceFolderPath(string assetPath)
        {
            return assetPath.Replace(REMOVE_FROM_PATH, "");
        }
        public static string GetResourcePath(string assetPath)
        {
            return GetResourceFolderPath(assetPath).Split('.')[0];
        }

#if UNITY_EDITOR
        public static string GetAssetPath(Object assetObject)
        {
            return GetResourcePath(AssetDatabase.GetAssetPath(assetObject));

        }
#endif
    }
}


