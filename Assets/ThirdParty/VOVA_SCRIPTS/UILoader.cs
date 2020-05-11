using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoader : MonoBehaviour {
    [SerializeField]
    Loader loader;
    [SerializeField]
    Slider slider;
    [SerializeField]
    GameObject rootCanvas;
    [SerializeField]
    float minimumLoadTime = 1;
   
    Coroutine coroutine;
    // Use this for initialization
    void Awake () {
        loader.StartSceneLoading += Enable;
        Disable();
    }

    protected void Enable()
    {   
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(UpdateRoutine());
    }
    protected void Disable()
    {
        rootCanvas.SetActive(false);

    }

    IEnumerator UpdateRoutine () {
        slider.value = 0;
        rootCanvas.SetActive(true);

        float startTime = Time.realtimeSinceStartup; ;
        while (slider.value < 1)
        {
            yield return null;
            float newSliderValue = Mathf.Lerp(slider.value, loader.LoadProgress, Time.deltaTime);
            slider.value = newSliderValue;
        }
        if (Time.realtimeSinceStartup - startTime < minimumLoadTime)
        {
            yield return new WaitForSeconds(minimumLoadTime - Time.realtimeSinceStartup - startTime);
        }
        else 

        yield return null;

        Disable();

    }
}
