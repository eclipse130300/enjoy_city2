using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseLoader : Singleton<BaseLoader>
{
    public float LoadProgress
    {

        get
        {
            float curentSceneLoading = 0;
            if(sceneLoading != null)
                curentSceneLoading = sceneLoading.progress;

            return  1 - (sceneToLoad.Count -1) / (loadedScenes.Count + curentSceneLoading) - ((afterLoading != null)?0.2f:0);
        }

    }
    public bool IsLoading
    {

        get
        {
            return sceneToLoad.Count > 0;

        }

    }

    [SerializeField]
    protected List<string> loadedScenes = new List<string>();
    [SerializeField]
    protected List<string> sceneToLoad = new List<string>();
    [SerializeField]
    protected List<string> sceneToUnload = new List<string>();
    [SerializeField]
    protected string curentLoadingScene = "" ;

    protected string activeScene = "";

    protected AsyncOperation sceneLoading;
    public SimpleActionEvent<string> SceneLoaded = new SimpleActionEvent<string>();
    public SimpleActionEvent StartSceneLoading = new SimpleActionEvent();
    public SimpleActionEvent AllSceneLoaded = new SimpleActionEvent();
    public SimpleActionEvent AllSceneUnloaded = new SimpleActionEvent();
    protected Coroutine loadAllScenesCoroutine;
    protected Coroutine unloadAllScenesCoroutine;

    public IEnumerator afterLoading;
    void Start()
    {

        Init();
    }
    protected virtual void Init()
    {
        loadedScenes.Clear();
        sceneToLoad.Clear();
        sceneToUnload.Clear();

      
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {

            if (!loadedScenes.Contains(SceneManager.GetSceneAt(i).name))
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

        }

    }

    public bool LoadScene(string sceneName, bool setActive = false)
    {
        if(setActive)
            activeScene = sceneName;

        if (!SceneManager.GetSceneByName(sceneName).isLoaded && !sceneToLoad.Contains(sceneName) && !loadedScenes.Contains(sceneName) && curentLoadingScene != sceneName)
        {
            if (Application.isPlaying)
            {
                sceneToLoad.Add(sceneName);
                if (loadAllScenesCoroutine == null)
                {
                    loadAllScenesCoroutine = StartCoroutine(LoadAllScenes()) ;
                }
            }
            else
            {
#if UNITY_EDITOR
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                foreach (var settingsScene in scenes) {
                    if (settingsScene.path.Contains(sceneName))
                    {
                        var scene = EditorSceneManager.OpenScene(settingsScene.path, OpenSceneMode.Additive);
                        if (setActive) {
                            EditorSceneManager.SetActiveScene(scene);
                        }

                        loadedScenes.Add(sceneName);
                        break;
                    }
                }
#endif
            }
            return true;
        }
        return false;
    }
    public void UnloadScene(string name)
    {
        if (Application.isPlaying)
        {

            if (!sceneToUnload.Contains(name) && (loadedScenes.Contains(name) || sceneToLoad.Contains(name)))
                sceneToUnload.Add(name);

            if (unloadAllScenesCoroutine == null)
                unloadAllScenesCoroutine = StartCoroutine(UnloadAllScenes());
            
        }
        else
        {
#if UNITY_EDITOR
            EditorSceneManager.CloseScene(SceneManager.GetSceneByName(name), true);
            loadedScenes.Remove(name);
#endif
        }
    }

    private IEnumerator LoadAllScenes()
    {
        Debug.LogError(" LoadAllScenes");
        StartSceneLoading.Invoke();
        while (sceneToLoad.Count != 0)
        {
            yield return unloadAllScenesCoroutine;
            sceneLoading = SceneManager.LoadSceneAsync(sceneToLoad[0], LoadSceneMode.Additive);
            curentLoadingScene = sceneToLoad[0];
            yield return sceneLoading;

            SceneLoaded.Invoke(sceneToLoad[0]);
            loadedScenes.Add(sceneToLoad[0]);
            sceneToLoad.RemoveAt(0);
        }
        if (activeScene != "")
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(activeScene));
        }
        yield return null;
        yield return afterLoading;
        afterLoading = null;
       
     
        curentLoadingScene = "";
        loadAllScenesCoroutine = null;
        sceneLoading = null;
        Debug.LogError(" AllSceneLoaded.Invoke(); 2");
        AllSceneLoaded.Invoke();
    }
    private IEnumerator UnloadAllScenes() {
        
        while (sceneToUnload.Count != 0)
        {
           // yield return loadAllScenesCoroutine;
            
            if (sceneToLoad.Contains(sceneToUnload[0]))
            {
                sceneToLoad.Remove(sceneToUnload[0]);
            }
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneToUnload[0]));
            loadedScenes.Remove(sceneToUnload[0]);
            sceneToUnload.RemoveAt(0);
        }
        unloadAllScenesCoroutine = null;
        AllSceneUnloaded.Invoke();
    }

}
