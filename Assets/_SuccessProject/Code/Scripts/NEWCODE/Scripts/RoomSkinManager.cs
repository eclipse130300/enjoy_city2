using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSkinManager : MonoBehaviour
{
    public FURNITURE _furniture;
    public RoomConfig currentConfig;
/*    public RoomConfig defaultConfig;*/


    private void Awake()
    {
        Messenger.AddListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, OnFurnitureChanged);
        /*        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, InitializeSkins);*/
        Messenger.AddListener(GameEvents.CLOTHES_CHANGED, InitializeSkins);
/*        SetDefaultConfig();*/
/*        Loader.Instance.AllSceneLoaded += InitializeSkins;*/
    }

    private void Start()
    {

        InitializeSkins();

    }

/*    private void SetDefaultConfig()
    {
        var cfg = new RoomConfig();

        foreach (string name in Enum.GetNames(typeof(FURNITURE)))
        {
            if (ScriptableList<RoomItemConfig>.instance.GetItemByID("default" + name) != null)
            {
                cfg.AddItemToConfig(ScriptableList<RoomItemConfig>.instance.GetItemByID("default" + name));
            }
        }

        defaultConfig = cfg;
    }*/

    private void OnFurnitureChanged(FURNITURE furniture)
    {
        _furniture = furniture;

        InitializeSkins();
    }



    // puts on real model
    private void InitializeSkins()
    {
        LoadConf();
/*        //PUT ON DEFAULT CLOTHES FIRST
        ApplyConfig(defaultConfig);*/
        //PUT ON CLOTHES FROM CONFIG
        ApplyConfig(currentConfig);
    }

    private void ApplyConfig(RoomConfig conf)
    {
        if (conf == null) return;

        foreach (string dirtyPair in conf.pickedItemsAndVariants)
        {
            string[] strs = dirtyPair.Split('+');
            var item = ScriptableList<RoomItemConfig>.instance.GetItemByID(strs[0]);
            if (item != null)
            {
                var privewingItems = FindObjectsOfType<IChangable>(); //just tag previewing gameobjects with this
                foreach (var it in privewingItems)
                {
                    if (it.gameObject.name == item.furnitureType.ToString())
                    {
                        var rotation = it.gameObject.transform.rotation; //save starting rotation


                        if (item.mesh != null)
                        {
                            it.gameObject.GetComponent<MeshFilter>().mesh = item.mesh;
                        }
                        it.gameObject.GetComponent<MeshRenderer>().material = item.material;
                        it.gameObject.GetComponent<MeshRenderer>().material.color = conf.GetActiveVariant(item).color;
                        it.transform.rotation = rotation;
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
/*        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, InitializeSkins);*/
        Messenger.RemoveListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, OnFurnitureChanged);
        Messenger.RemoveListener(GameEvents.CLOTHES_CHANGED, InitializeSkins);

/*        Loader.Instance.AllSceneLoaded -= InitializeSkins;*/
    }

    void LoadConf()
    {
        currentConfig = SaveManager.Instance.LoadRoomSet();
    }

}
