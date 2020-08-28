using CMS.Config;
using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{

    public BodyConfig currentBodyConfig;
    private SaveManager saveManager;
    private SkinsManager skinsManager;
    private PhotonView photon;
/*
    private bool IsSpawned { get { return currentBodyConfig != null; } }*/

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        skinsManager = GetComponent<SkinsManager>();

        photon = GetComponent<PhotonView>();
    }


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            if (photon.IsMine) //we spawn body for our player
            {
                currentBodyConfig = saveManager.LoadBody();
                ApplyBodyConfig(currentBodyConfig);
            }
            else if (!photon.IsMine && photon.Owner != null) //we spawn other player bodies
            {
                var playerProps = photon.Owner.CustomProperties;
                string PlayerBodySTR = playerProps["playerWithTeam"].ToString();
                PaintBallPlayer player = JsonConvert.DeserializeObject<PaintBallPlayer>(PlayerBodySTR);
                ApplyBodyConfig(player.bodyConfigID);
            }
        }
        //we just spawn locally
        else if(saveManager.LoadBody() != null)
        {
            currentBodyConfig = saveManager.LoadBody();
            ApplyBodyConfig(currentBodyConfig);
        }
    }

    private void ApplyBodyConfig(string configID)
    {
        var body  = ScriptableList<BodyConfig>.instance.GetItemByID(configID);
        ApplyBodyConfig(body);
    }

    private void ApplyBodyConfig(BodyConfig bodyCfg)
    {
        //first delete all children
        var allChildren = transform.GetComponentsInChildren<IBodyPrefab>();
        foreach(IBodyPrefab child in allChildren)
        {
            child.gameObject.SetActive(false);
        }

        //than instantiate pref
        var body = Instantiate(bodyCfg.game_body_prefab);

        body.transform.SetParent(gameObject.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.identity;

        //apply other stuff
        var animator = body.GetComponent<Animator>();
        if (animator == null) body.AddComponent<Animator>();

        animator.runtimeAnimatorController = bodyCfg.controller;
        animator.avatar = bodyCfg.avatar;
        animator.applyRootMotion = false;

        if(body.GetComponent<MecanimWrapper>())  body.GetComponent<MecanimWrapper>().animator = animator;

        skinsManager.skinHolder = body.transform;
        skinsManager.PutOnClothes();
    }

    private void OnDestroy()
    {
    }
}
