using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOnSceneLoad : MonoBehaviour
{
    public string sceneNameToAppear;

    private void OnEnable()
    {
        Loader.Instance.AllSceneLoaded += Toggle;
    }

    private void OnDisable()
    {
        Loader.Instance.AllSceneLoaded -= Toggle;
    }

    private void Toggle()
    {
        bool value = Loader.Instance.InterfaceSceneName == sceneNameToAppear ? true : false;
        gameObject.SetActive(value);
    }
}
