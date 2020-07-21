using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewInputManager : MonoBehaviour
{
    [SerializeField] MecanimWrapper mecanimWrapper;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mecanimWrapper.SetVerticalSpeed(0);
        mecanimWrapper.SetHorizontalSpeed(0);
    }
}
