﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapBorderClumper : MonoBehaviour
{
    private Camera minimapCam;
    private ClumpedIcon[] clumpedIcons;
    public float iconsOffset = 15f; //resize offset from player to clumped icon



    private void Awake()
    {
        minimapCam = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
        clumpedIcons = FindObjectsOfType<ClumpedIcon>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraXZ = new Vector3(minimapCam.transform.position.x, 0, minimapCam.transform.position.z);

        foreach (ClumpedIcon icon in clumpedIcons)
        {
            Vector3 entryPointPos = icon.transform.parent.position;
            Vector3 entryPointXZ = new Vector3(entryPointPos.x, 0, entryPointPos.z);
            float distance = Vector3.Distance(cameraXZ, entryPointXZ);
            if(distance < iconsOffset)
            {
                icon.transform.position = entryPointPos;
            }
            else
            {
                Vector3 dir = (entryPointXZ - cameraXZ).normalized;
                Vector3 clumpedIconV = cameraXZ + dir * (iconsOffset);
                clumpedIconV.y = entryPointPos.y;
                icon.transform.position = clumpedIconV;
            }
        }


    }
}
