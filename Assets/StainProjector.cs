using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StainProjector : MonoBehaviour
{

    [SerializeField] float dissapearTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestroyImmediate", dissapearTime);
    }

    void SelfDestroyImmediate()
    {
        gameObject.SetActive(false);
    }
}
