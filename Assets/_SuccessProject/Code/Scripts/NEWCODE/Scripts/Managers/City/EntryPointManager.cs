﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class EntryPointManager : MonoBehaviour
{
    [SerializeField] private int checkPointsPerFrame;
    [SerializeField] private float distanceToTrigger;

    private EntryPoint activePoint;
    private PlayerLevel player;
    private Queue<EntryPoint> allEntryPoints = new Queue<EntryPoint>();


    private void Start()
    {
        if (PhotonView.Get(this) != null && !PhotonView.Get(this).IsMine && PhotonNetwork.IsConnectedAndReady) {
            this.enabled = false;
        
        }
    }
    // Start is called before the first frame update

    private void OnEnable()
    {
        Loader.Instance.AllSceneLoaded += Initialize;
    }

    private void OnDisable()
    {
        if(Loader.Instance != null)   Loader.Instance.AllSceneLoaded -= Initialize;
        if (activePoint != null) activePoint.UnlistenInteractionButton();
    }

    private void Initialize()
    {
        player = GetComponent<PlayerLevel>();
        var points = FindObjectsOfType<EntryPoint>();

        foreach (EntryPoint pt in points)
        {
            allEntryPoints.Enqueue(pt);
        }

        StartCheck();

        if (!allEntryPoints.IsNullOrEmpty())
            StartCoroutine(ContinuousCheck());

    }

    EntryPoint ActivePointCheck(EntryPoint point)
    {
        float distance = Vector3.Distance(player.transform.position, point.transform.position);
        if (distance <= distanceToTrigger && LvlCheck(player.level, point.fromLevelAvailable))
        {
            return point;
        }
        else
        {
            return null;
        }
    }

    private bool StartCheck()
    {
        foreach( EntryPoint point in allEntryPoints)
        {
            if (ActivePointCheck(point))
            {
                point.ShowUI();
                point.ListenInteractionButton();
/*                Debug.Log("POINT" + point.name + "SHOWN AFTER START CHECK");*/
                return true;
            }
        }
        return false;
    }

    IEnumerator ContinuousCheck()
    {

        while (true)
        {
            for (int i = 0; i < checkPointsPerFrame; i++)
            {
                var point = allEntryPoints.Dequeue();
                if (ActivePointCheck(point) && activePoint == null && WallCastCheck(player.transform, point.transform))
                {
                    point.ShowUI();
                    point.ListenInteractionButton();
/*                    Debug.Log("Show");*/

                    activePoint = point;
                }
                else if (ActivePointCheck(point) == null)
                {
                    if (point == activePoint)
                    {
                        point.HideUI();
                        point.UnlistenInteractionButton();
/*                        Debug.Log("Hide");*/

                        activePoint = null;
                    }
                }
                allEntryPoints.Enqueue(point);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    bool LvlCheck(int plyaerLvl, int lvlToTrigger)
    {
        bool result;
        result = plyaerLvl >= lvlToTrigger ? true : false;
        return result;
    }

    bool WallCastCheck(Transform player, Transform point)
    {
        Vector3 dir = player.position - point.position;
        Ray ray = new Ray(point.position, dir);

/*        Debug.DrawRay(point.position, dir);*/

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            bool reuslt = hit.collider.transform.parent?.GetComponent<PlayerLevel>() ; // collider parent! delete with new playermodel
            return reuslt;
        }
        return false;
    }

}
