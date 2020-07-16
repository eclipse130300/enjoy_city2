using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{

    public BodyConfig currentBodyConfig;
    private SaveManager saveManager;
    private SkinsManager skinsManager;

    private bool bodyIsSpawned;
    private void Awake()
    {
        saveManager = SaveManager.Instance;
        skinsManager = GetComponent<SkinsManager>();


        Loader.Instance.AllSceneLoaded += Initialize;
    }

 
    // Start is called before the first frame update
/*    void Start()
    {
        Initialize();
    }*/

    private void Initialize()
    {
        /*if (currentBodyConfig != null)
            return;*/

        /*currentBodyConfig = null;*/

        // loads config from save manager and applies it
        if (saveManager.LoadBody() != null && !bodyIsSpawned)
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

    private void ApplyBodyConfig(BodyConfig config)
    {
        Debug.Log("CALL - APPLYBODY");
        //first delete all children
        var allChildren = transform.GetComponentsInChildren<IBodyPrefab>();
        foreach(IBodyPrefab child in allChildren)
        {
            child.gameObject.SetActive(false);
        }

        //than instantiate pref
        var body = Instantiate(config.game_body_prefab);
        body.transform.SetParent(gameObject.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.identity;

        //apply other stuff
        var animator = body.GetComponent<Animator>();
        if (animator == null) body.AddComponent<Animator>();
        animator.runtimeAnimatorController = config.controller;
        animator.avatar = config.avatar;
        animator.applyRootMotion = false;

        GetComponent<MecanimWrapper>().animator = animator;

        skinsManager.skinHolder = body.transform;
        skinsManager.InitializeSkins();

        bodyIsSpawned = true;
    }

    private void OnDestroy()
    {
        Loader.Instance.AllSceneLoaded -= Initialize;
    }
}
