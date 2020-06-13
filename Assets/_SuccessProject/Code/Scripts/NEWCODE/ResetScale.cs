using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScale : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = Vector3.one;
    }
}
