﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShoter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ScreenCapture.CaptureScreenshot("Assets/" + Random.Range(0, 300) + "screen.png");
    }

}
