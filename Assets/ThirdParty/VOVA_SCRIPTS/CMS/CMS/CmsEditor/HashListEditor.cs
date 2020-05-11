using CMS.Config;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

#if UNITY_EDITOR
using UnityEditor;
namespace CMS.Editor
{
    public class HashListEditor : EditorWindow
    {
        public void OnGUI()
        {
            if (GUILayout.Button("Recalculate hashes", GUILayout.ExpandWidth(false)))
            {
                RecalckHashes();
            }

            if (GUILayout.Button("Post to Db", GUILayout.ExpandWidth(false)))
            {
#if UNITY_EDITOR
                /*Network.DBEntityFramework.MigrationManager.UpdateDb("Data Source=127.0.0.1;User ID=sa;Password=1234567890;Initial Catalog=KazakiMainDB;");*/
#endif
            }
        }
        public void RecalckHashes()
        {
            int i = 0;
            string[] derictories = Directory.GetDirectories(Application.dataPath + "/Resources/Configs");
            string[] configFiles = Directory.GetFiles(Application.dataPath + "/Resources/Configs", "*.asset", SearchOption.AllDirectories);
            ScriptableList<BaseScriptableDrowableItem>.configHash.hashes.Clear();
            foreach (string configFile in configFiles)
            {

                string assetPath = configFile.Replace(Application.dataPath + "/Resources/", "").Replace('\\', '/').Replace(".asset", "");
                ConfigHash _configHash = AssetDatabase.LoadAssetAtPath(assetPath, typeof(ConfigHash)) as ConfigHash;
                BaseScriptableDrowableItem source = Resources.Load<BaseScriptableDrowableItem>(assetPath);

                if (source != null)
                {
                   
                    string json = JsonConvert.SerializeObject(source, Formatting.Indented,
                                    new JsonSerializerSettings()
                                    {
                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                                        
                                        NullValueHandling = NullValueHandling.Ignore,
                                        MissingMemberHandling = MissingMemberHandling.Ignore,
                                        //Error = HandleDeserializationError,
                                        //PreserveReferencesHandling = PreserveReferencesHandling.All
                                    }
                                );
                   // Debug.Log(json);
                    
                    ScriptableList<BaseScriptableDrowableItem>.SetNewHash(i,assetPath, json);
                    i++;


                }
                

            }

            AssetDatabase.SaveAssets();

        }

        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
          // e.ErrorContext.Handled = true;

        }
    }
}

#endif