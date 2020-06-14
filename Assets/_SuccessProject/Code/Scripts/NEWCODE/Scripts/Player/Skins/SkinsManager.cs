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
    public ClothesConfig defaultConfig;

    public PhotonView photon;

    //public static Player =
    private void Awake()
    {
        _gameMode = Loader.Instance.curentScene.gameMode;

        photon =  PhotonView.Get(this);
        if (GetComponent<PreviewManager>() != null /*&& */) //ckeck if we are in character editor
        {
           
            Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
      
        }
        if ((photon == null || photon.IsMine || !PhotonNetwork.IsConnectedAndReady)) {
            Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
            SetDefaultConfig();
        }

    }

    private void SetDefaultConfig()
    {
        var cfg = new ClothesConfig();

        var allDefaultItems = ScriptableList<ItemConfig>.instance.list.Where(t => t.isDefault).ToList();

        foreach (ItemConfig defaultItem in allDefaultItems)
        {
            if (defaultItem != null)
            {
                cfg.AddItemToConfig(defaultItem, defaultItem.variants[0]);
            }
        }
        defaultConfig = cfg;
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        _gameMode = gameMode;
        currentConfig = LoadConf(_characterSex, _gameMode);
        PutOnClothes(currentConfig);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
     
    }
    private void Start()
    {
        PutOnClothes();

    }

        // puts on real model
        private void PutOnClothes()
        {
        
            if (photon == null || photon.IsMine || !PhotonNetwork.IsConnectedAndReady)
            {
                currentConfig = LoadConf(_characterSex, _gameMode);
                PutOnClothes(currentConfig);
               
            } else
    
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
        Debug.Log("OnPlayerPropertiesUpdate" + changedProps["skin"]);
        if (targetPlayer == photon.Owner && !photon.IsMine)
            PutOnClothes(changedProps["skin"].ToString());
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
       
       
       
    }
    private void PutOnClothes(ClothesConfig config)
    {
        //PUT ON DEFAULT CLOTHES FIRST
        foreach (string dirtyPair in defaultConfig.pickedItemsAndVariants)
        {
            string[] strs = dirtyPair.Split('+');
            var item = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);
            var bodyTransform = skinHolder.Find(item.bodyPart.ToString());
            bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = item.mesh;
            bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        }
        //PUT ON CLOTHES FROM CONFIG
        if (config != null)
        {
            foreach (string dirtyPair in config.pickedItemsAndVariants)
            {
               
                string[] strs = dirtyPair.Split('+');
                var item = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);
                if (item != null)
                {
/*                    Debug.Log("dirtyPair " + dirtyPair);*/
                    var bodyTransform = skinHolder.Find(item.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART
                    bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = item.mesh;

                    bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = config.GetActiveVariant(item).color;
                }
            }
        }
        if (photon!= null && photon.IsMine)
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
            Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
            SetDefaultConfig();
        }
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
public enum Gender
{
    MALE,
    FEMALE
}
