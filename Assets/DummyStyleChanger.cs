using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyStyleChanger : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer boots;
    [SerializeField] private SkinnedMeshRenderer head;


    private void Awake()
    {
        Messenger.AddListener<ItemConfig>(GameEvents.PUT_ON_ITEM, OnItemPressed);
    }

    private void OnItemPressed(ItemConfig itemCFG)
    {
        Debug.Log("I take config and DO SOMETHING WITH IT");
        boots.sharedMesh = itemCFG?.prefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<ItemConfig>(GameEvents.PUT_ON_ITEM, OnItemPressed);
    }
}
