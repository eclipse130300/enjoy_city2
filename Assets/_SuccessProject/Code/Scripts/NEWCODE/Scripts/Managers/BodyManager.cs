﻿using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{

    public BodyConfig currentBodyConfig;
    private SaveManager saveManager;
    private SkinsManager skinsManager;

    private bool IsSpawned { get { return currentBodyConfig != null; } }

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        skinsManager = GetComponent<SkinsManager>();

    }


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // loads config from save manager and applies it
        if (saveManager.LoadBody() != null || !IsSpawned)
        {
            currentBodyConfig = saveManager.LoadBody();
            ApplyBodyConfig(currentBodyConfig);
        }
        else
        {
            Debug.LogError("Cannot find body_config in saveManager...");
            // if cannot find one, add default config to save manager 
            // applies default config
        }
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

        body.GetComponent<MecanimWrapper>().animator = animator;

        skinsManager.skinHolder = body.transform;
        skinsManager.InitializeSkins();

        Debug.Log("I SPAWNED BODY!");
    }

    private void OnDestroy()
    {
    }
}
