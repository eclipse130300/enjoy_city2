using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;
using System.Reflection;
using System.Linq;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CMS.Config
{
    public class ScriptableList<T> where T : BaseScriptableDrowableItem, IGUIDrawable
    {
        private List<T> _list = new List<T>();

        private List<string> _idList = new List<string>();

        private static ScriptableList<T> _instance;

        public static ScriptableList<T> instance
        {
            get
            {
                if (_instance == null)
                {

                    _instance = new ScriptableList<T>();
                }
                return _instance;
            }
        }
        public List<T> list
        {
            get
            {
                return _list;
            }

        }

        public T this[int index]
        {
            set
            {
                list[index] = value;
            }

            get
            {
                return list[index];
            }
        }
        public static string configPath
        {
            get
            {
                return "Assets/Resources/Configs";
            }
        }
        public string path
        {
            get
            {
                return configPath + "/" + typeName;
            }
        }
        public string typeName
        {
            get
            {
                return typeof(T).ToString();
            }
        }
        private static ConfigHash _configHash;

        public static ConfigHash configHash
        {
            get
            {
                if (_configHash == null)
                    _configHash = Resources.Load<ConfigHash>("Configs/ConfigHash");
#if UNITY_EDITOR
                if (_configHash == null)
                {

                    CreateHashList();
                }
#endif
                return _configHash;
            }

        }

        public List<string> GetIdList(Predicate<T> match = null)
        {
          instance.  _idList.Clear();
            instance.list.ForEach(delegate (T element)
            {
                if (match != null)
                {
                    if (match(element))
                    {
                        instance._idList.Add(element.ConfigId);
                    }
                }
                else
                {
                    instance._idList.Add(element.ConfigId);
                }
            });
            return instance._idList;
        }
        public ScriptableList()
        {
            if (list.Count == 0)
            {
                _instance = this;
                instance.LoadList();
            }
        }
        public T GetItemByID(string id)
        {
            T item = instance.list.Find((x) => { return x.ConfigId == id; });
            return item;
        }

        public T GetItemByID(int id)
        {
            T item = instance.list.Find((x) => { return x.ConfigIntId == id; });
            return item;
        }

        public bool CheckNameValid(string id, int cIndex)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name == id && i != cIndex)
                {
                    return false;
                }
            }
            return true;
        }
        private void LoadList()
        {
           
#if UNITY_EDITOR
            LoadListEditor();
#else
           LoadListInGame();
#endif

        }
        private void LoadListInGame()
        {
            _list = new List<T>( Resources.LoadAll<T>(AssetDatabaseUtil.GetResourceFolderPath(path)));
            _list.RemoveAll(x=>x.removedFromList);
        }

        #region Editor
#if UNITY_EDITOR

        public static IEnumerable<Type> GetAllConfigClasses()
        {


            return  Assembly.GetAssembly(typeof(BaseScriptableDrowableItem))
                            .GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(BaseScriptableDrowableItem)));
        }
        public static void CreateHashList()
        {
            Debug.Log("_configHash == null");
            _configHash = ScriptableObject.CreateInstance<ConfigHash>();
            _configHash.name = "ConfigHash";

            AssetDatabase.CreateAsset(configHash, configPath + "/" + configHash.name + ".asset");
            AssetDatabase.SaveAssets();

        }
        public void LoadListEditor()
        {

            if (!AssetDatabase.IsValidFolder(configPath))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Configs");
            }
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(configPath, typeof(T).ToString());
            }

            string[] configFiles = Directory.GetFiles(Application.dataPath + path.Replace("Assets", ""), "*.asset", SearchOption.AllDirectories);
            foreach (string configFile in configFiles)
            {

                string assetPath = path + configFile.Replace(Application.dataPath + path.Replace("Assets", ""), "").Replace('\\', '/');
                T source = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
	            if (source != null)
	            {
		            if (!source.removedFromList)
		            {
			            list.Add(source);
		            }
	            }
	            else
	            {
		            Debug.LogError($"Invalid load '{assetPath}'");
	            }
			}
        }

        public T AddItem(string name = "")
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.name = name == "" ? typeName + list.Count : name;
            list.Add(instance);


            AssetDatabase.CreateAsset(instance, path + "/" + instance.name + ".asset");
            AssetDatabase.SaveAssets();


            return instance;
        }

        public void DeleteItem(int index, bool removeFromDisk)
        {
            if (index < list.Count)
            {
                if (removeFromDisk)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(list[index]));
                }
                else
                {
                    list[index].removedFromList = true;
                }
                list.RemoveAt(index);

            }
            else
            {

            }
        }
        public void SaveAllNewItem()
        {
            AssetDatabase.SaveAssets();
        }
        public static void SaveNewConfigFile(int id, string path, string hash, Byte[] file)
        {
            AssetBundle buindle = AssetBundle.LoadFromMemory(file);
            UnityEngine.Object[] objects = buindle.LoadAllAssets();
            AssetDatabase.CreateAsset(objects[0], path);
            SetNewHash(id, path, hash);
        }

        public static void SetNewHash(int id, string path, string hash)
        {
            IntStringString pair = configHash.hashes.Find(x => x.Value2 == path);
            if (pair == null)
            {
                IntStringString newPair = new IntStringString();
                newPair.Value1 = id;
                newPair.Value2 = path;
                newPair.Value3 = hash;
                configHash.hashes.Add(newPair);
            }
            else
            {
                pair.Value3 = hash;
            }
            EditorUtility.SetDirty(configHash);
        }
        public static bool CheskHash(string path, string hash)
        {

            var element = configHash.hashes.Find(x => x.Value2 == path);
            return element != null && element.Value3 == hash;

        }

        public T GetOrCreateItem(string id)
        {
            T item = GetItemByID(id);

            if (item == null)
            {
                item = AddItem(id);
            }
            return item;
        }
#endif
        #endregion
    }
}
