using CMS.Editor;
using Newtonsoft.Json;
using System;
using System.Timers;
using UnityEditor;

using UnityEngine;
namespace CMS.Config
{
    [System.Serializable]
    public class BaseScriptableDrowableItem : ScriptableObject, IGUIDrawable, IScriptableListItem, ICloneable
    {
        [SerializeField]
        int configIntId = 0;
        public int ConfigIntId
        {
            get
            {
                return configIntId;
            }
            set
            {
                configIntId = value;
            }
        }
        public string ConfigId
        {
            get {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(newName))
                    newName = name;
                return newName;

#else
                return name;
#endif
            }
            set {

#if UNITY_EDITOR
                if(value != "")
                    newName = value;
#endif
                
            }
        }

        [Draw(DrawAttributeTypes.NotForDraw, "")]
        public bool removedFromList;

        public object Clone()
        {
            return ScriptableObject.Instantiate(this);
            return this.MemberwiseClone();
        }
        #region Editor
#if UNITY_EDITOR
        string newName = String.Empty;
        Timer timer ;
        public virtual void Draw()
        {
            ConfigId = EditorGUILayout.DelayedTextField("ID", ConfigId);

            ConfigIntId = EditorGUILayout.DelayedIntField("Server ID", ConfigIntId);
        }
        public virtual void Save()
        {
            if (name != newName && newName != "")
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), newName);
                AssetDatabase.SaveAssets();
            }
 
            EditorUtility.SetDirty(this);
        }
        public virtual GUIContent ContentForListMinimal()
        {
            return new GUIContent(name);
        }
        public virtual GUIContent ContentForListMaximal()
        {
            return new GUIContent(name);
        }
        public virtual int IsContains(string searchRequest)
        {
            return name.ToLower().IndexOf(searchRequest.ToLower());
        }

#endif
        #endregion
    }

}
