using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;

public class ToggleOnSceneLoad : MonoBehaviour
{
    public string sceneNameToAppear;
/*    public MapConfig mapCFGToAppear;*/

    private void Awake()
    {
        Loader.Instance.AllSceneLoaded += Toggle;
    }

    private void OnDestroy()
    {
        Loader.Instance.AllSceneLoaded -= Toggle;
    }

    private void Toggle()
    {
        bool value = Loader.Instance.InterfaceSceneName == sceneNameToAppear ? true : false;
        gameObject.SetActive(value);
    }
}
