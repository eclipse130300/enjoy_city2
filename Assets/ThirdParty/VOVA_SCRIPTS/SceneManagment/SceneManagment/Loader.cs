using CMS.Config;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
[ExecuteInEditMode]
public class Loader : BaseLoader
{
    public new static Loader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Loader)FindObjectOfType(typeof(Loader));

                if (FindObjectsOfType(typeof(Loader)).Length > 1)
                {
                    return _instance as Loader;
                }
            }

            return _instance as Loader;

        }
    }

    public MapConfig deafaultScene;
    [SerializeField]
    private string sceneName ="";
    public string SceneName {
        get
        {
            return sceneName;
        }
        set
        {

            UnloadScene(sceneName);
            sceneName = value;
            if (!string.IsNullOrEmpty(value))
                LoadScene(value, true);
            
        }
    }
    [SerializeField]
    private string interfaceSceneName ="";
    public string InterfaceSceneName
    {
        get
        {
            return interfaceSceneName;

        }
        set
        {
            UnloadScene(interfaceSceneName);
            interfaceSceneName = value;
            if (!string.IsNullOrEmpty(value))
                LoadScene(value);
        }
    }
    [SerializeField]
    private string controllersSceneName = "";
    public string ControllersSceneName
    {
        get
        {
            return controllersSceneName;

        }
        set
        {
            UnloadScene(controllersSceneName);
            controllersSceneName = value;
            if (!string.IsNullOrEmpty(value))
                LoadScene(value);
        }
    }
   // public MapConfig mapConfig;
    protected override void Init() {
        base.Init();

        if (sceneToLoad.Count == 0 && SceneManager.sceneCount > 1)
        {
            /*
            if (mapConfig)
            { 
                
                if(SceneName != mapConfig.SceneName)
                    this.SceneName = mapConfig.SceneName;
                if(InterfaceSceneName != mapConfig.InterfaceSceneName)
                    this.InterfaceSceneName = mapConfig.InterfaceSceneName;
                if (ControllersSceneName != mapConfig.ManagersSceneName)
                    this.ControllersSceneName = mapConfig.ManagersSceneName;
            }*/

            AllSceneLoaded.Invoke();
        }
        if(SceneManager.sceneCount == 1 && Application.isPlaying && deafaultScene != null)
            (Loader.Instance as Loader).LoadGameScene(deafaultScene); 
    }
    public static void LoadLoaderSceneAdditive(LoadSceneMode mode)
    {
        for(int i=0;i< SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == "Scenes/Loader")
            {
                return;
            }
        }
        SceneManager.LoadScene("Scenes/Loader", LoadSceneMode.Additive);

    }

    public void LoadGameScene(MapConfig config)
    {
       // mapConfig = config;
        this.SceneName = config.SceneName;
        this.ControllersSceneName = config.ManagersSceneName;
        this.InterfaceSceneName = config.InterfaceSceneName;

    }
    public void LoadGameScene(string SceneName, string InterfaceSceneName, string ControllersSceneName)
    {
        this.SceneName = SceneName;
        this.ControllersSceneName = ControllersSceneName;
        this.InterfaceSceneName = InterfaceSceneName;

    }
#if UNITY_EDITOR
    /// <summary>
    /// EDITOR ONLY
    /// </summary>
    [MenuItem("Loader/Open %#l ")]
    public static void LoadLoaderScene()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].path.Contains("Loader"))
            {

                var scene = EditorSceneManager.OpenScene(scenes[i].path, OpenSceneMode.Single);
                break;
            }
        }
       // (Loader.Instance as Loader).mapConfig = null;
    }
    /// <summary>
    /// EDITOR ONLY
    /// </summary>
    [MenuItem("Loader/TestGame %#l ")]
    public static void LoadTestGame()
    {
        (Loader.Instance as Loader).LoadGameScene(ScriptableList<MapConfig>.instance.GetItemByID("City_2"));
    }
    /// <summary>
    /// EDITOR ONLY
    /// </summary>
    [MenuItem("Loader/Start Game %#k ")]
    public static void StartGame()
    {
        (Loader.Instance as Loader).LoadGameScene(ScriptableList<MapConfig>.instance.GetItemByID("Lobby"));
    }

#endif


}
