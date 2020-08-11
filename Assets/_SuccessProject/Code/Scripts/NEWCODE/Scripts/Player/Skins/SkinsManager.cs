using CMS.Config;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Newtonsoft.Json;
using PlayFab.MultiplayerModels;
using Photon.Realtime;
using System.Collections;


// set as a component of parent of modelBodyparts
public class SkinsManager :  MonoBehaviourPunCallbacks, IPunObservable//TODO MAKE UPDATE DEPENDING ON THE GENDER
{
    public Transform skinHolder;
    public GameMode _gameMode;
    public Gender _characterSex;
    public ClothesConfig currentConfig;

    public PhotonView photon;

    //public static Player =
    private void Awake()
    {
        _gameMode = Loader.Instance.curentScene.gameMode;
        photon =  PhotonView.Get(this);

        _characterSex = SaveManager.Instance.LoadBody().gender; //take body gender (we choose it at the first launch)

        if (photon == null) return;

        if ((/*photon == null || */photon.IsMine || !PhotonNetwork.IsConnectedAndReady))
        {
            Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
            Messenger.AddListener(GameEvents.CLOTHES_CHANGED, InitializeSkins);
/*            Messenger.AddListener<Transform>(GameEvents.BODY_CHANGED, OnBodyChanged);*/
            if (GetComponent<PreviewManager>() != null /*&& */) //ckeck if we are in character editor
            {

                Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);

            }
        }
        else 
        {
            PutOnClothes();
        }

       
    }

    public void InitializeFields(Transform skinHolder)
    {
        this.skinHolder = skinHolder; 
    }

    public void InitializeSkins()
    {
        //getSex From savemanager
        currentConfig = LoadConf(_characterSex, _gameMode);

        //PUT ON CLOTHES FROM CONFIG
        PutOnClothes(currentConfig);
    }

/*    void OnBodyChanged(Transform transform)
    {
        skinHolder = transform;
    }*/

    private void OnGameModeChanged(GameMode gameMode)
    {
        _gameMode = gameMode;
        currentConfig = LoadConf(_characterSex, _gameMode);
        PutOnClothes(currentConfig);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
     
    }

/*    private void Start()
    {
        PutOnClothes();
    }*/

    // puts on real model

    private void PutOnClothes()
        {
        
            if (photon == null || photon.IsMine || !PhotonNetwork.IsConnectedAndReady)
            {
                currentConfig = LoadConf(_characterSex, _gameMode); //ERROR HERE!
                PutOnClothes(currentConfig);
            } 
            else
            {
               // Debug.Log("   Other         " + photon.Owner.UserId + " "+photon.Owner.CustomProperties["skin"].ToString());
               if(photon.Owner.CustomProperties["skin"] != null)
                PutOnClothes(photon.Owner.CustomProperties["skin"].ToString());
            }

        }
    public void PutOnClothes(string config)
    {
        PutOnClothes(JsonConvert.DeserializeObject<ClothesConfig>(config));
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (photon == null) return;

/*        Debug.Log("OnPlayerPropertiesUpdate" + changedProps["skin"]);*/
        if (targetPlayer == photon.Owner && !photon.IsMine)
            PutOnClothes(changedProps["skin"].ToString());
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);  

    }
    public void PutOnItem(ItemConfig config,ItemVariant variant) {
        if (config.itemObject != null)
        {
            if (skinHolder == null) return;

            var bodyTransform = skinHolder?.Find(config.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART

            var newBodyPart = GameObject.Instantiate(config.itemObject, skinHolder);
            newBodyPart.name = bodyTransform.name;
            GameObject.DestroyImmediate(bodyTransform.gameObject);

/*            if (config.mesh != null)
            newBodyPart.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = config.mesh;*/

            newBodyPart.GetComponentInChildren<SkinnedMeshRenderer>().material.color = variant.color;
            //and we disable
            DisableBodyParts(config.partsToDisable);
        }
        else
        {
            if (skinHolder != null && config.mesh != null/* && config.material != null*/)
            {
                var bodyTransform = skinHolder.Find(config.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART
                bodyTransform.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = config.mesh;
/*                bodyTransform.GetComponentInChildren<SkinnedMeshRenderer>().material = config.material;*/
                bodyTransform.GetComponentInChildren<SkinnedMeshRenderer>().material.color = variant.color;
            }
            else
            {
                Debug.LogWarning("Set mesh/material in config and check skinholder!");
            }
        }
    }

    private void DisableBodyParts(List<BodypartToDisable> bodypartsToDisable)
    {
        if (bodypartsToDisable == null) return;

        //disable bodyparts from config
        foreach (BodypartToDisable bodyParttoDisable in bodypartsToDisable)
        {
            Transform bpTransform = skinHolder.Find(bodyParttoDisable.ToString());
            bpTransform.gameObject.SetActive(false);
/*            Debug.Log("disable :" + bpTransform.gameObject.name);*/
        }
    }


    public void PutOnClothes(ClothesConfig config)
    {
    
        //PUT ON CLOTHES FROM CONFIG
        if (config != null)
        {
            foreach (string dirtyPair in config.pickedItemsAndVariants)
            {
               
                string[] strs = dirtyPair.Split('+');
                var item = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);

                if (item != null)
                {
                    PutOnItem(item, config.GetActiveVariant(item));
                }
            }
        }

        if (photon != null && photon.IsMine && photon.Owner != null)
        {
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties.Add("skin", JsonConvert.SerializeObject(config));
            photon.Owner.SetCustomProperties(customProperties);
           
        }
       
    }

    private void OnDestroy()
    {
    
        if (GetComponent<PreviewManager>() != null) //ckeck if this manager is prewiew skin manager
        {
           // Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
            Messenger.RemoveListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
        }

        if ((photon == null || photon.IsMine || !PhotonNetwork.IsConnectedAndReady))
        {
            Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
        }
        Messenger.RemoveListener(GameEvents.CLOTHES_CHANGED, InitializeSkins);
/*        Messenger.AddListener<Transform>(GameEvents.BODY_CHANGED, OnBodyChanged);*/

/*        if (Loader.Instance != null)
        Loader.Instance.AllSceneLoaded -= InitializeSkins;*/
    }

    ClothesConfig LoadConf(Gender gender, GameMode gameMode)
    {
        string key = gender.ToString() + gameMode.ToString();
        return SaveManager.Instance.LoadClothesSet(key); 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}


