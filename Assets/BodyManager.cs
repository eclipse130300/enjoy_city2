using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{

    public BodyConfig currentBodyConfig;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // loads config from save manager and applies it
        // if cannot find one, add default config to save manager 
        // apply default config
        ApplyBodyConfig(currentBodyConfig);
    }

    private void ApplyBodyConfig(BodyConfig config)
    {
        //first delete all children
        var allChildren = transform.GetComponentsInChildren<IBodyPrefab>();
        foreach(IBodyPrefab child in allChildren)
        {
            child.gameObject.SetActive(false);
        }

        //than instantiate pref
        var body = Instantiate(config.body_prefab);
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

        Messenger.Broadcast(GameEvents.GENDER_CHANGED, config.gender);
        Messenger.Broadcast(GameEvents.BODY_CHANGED, body.transform);
    }
}
