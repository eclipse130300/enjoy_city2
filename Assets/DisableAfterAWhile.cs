using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAWhile : MonoBehaviour
{
    public int secoundsToDisable;

    private void OnEnable()
    {
        StartCoroutine(DisableRoutine(secoundsToDisable));
    }

    IEnumerator DisableRoutine(int delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
