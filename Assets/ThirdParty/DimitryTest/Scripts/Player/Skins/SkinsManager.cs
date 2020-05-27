using CMS.Config;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkinsManager : MonoBehaviour //TODO MAKE UPDATE DEPENDING ON THE GENDER
{
    public GameMode _gameMode;
    public Gender _characterSex;
    public ClothesConfig currentConfig;
    public ClothesConfig defaultConfig;


    private void Awake()
    {

        if (GetComponent<PreviewManager>() != null) //ckeck if we are in editor
        {
            Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
        }
        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
        SetDefaultConfig();
    }

    private void SetDefaultConfig()
    {
        var cfg = new ClothesConfig();

        foreach (string name in Enum.GetNames(typeof(BODY_PART)))
        {
            if (ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + name) != null)
            {
                /*cfg.pickedItemsAndVariants.Add(ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + name), "default");*/
                cfg.AddItemToConfig(ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + name));
            }
        }

        defaultConfig = cfg;
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        _gameMode = gameMode;

        PutOnClothes();
    }

    // set as a component of parent of modelBodyparts
    private void Start()
    {
        PutOnClothes();
    }

    // puts on real model
    private void PutOnClothes()
    {
        LoadConf();

        foreach (string dirtyPair in defaultConfig.pickedItemsAndVariants)
        {
            string[] strs = dirtyPair.Split('+');
            var item = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);
            var bodyTransform = transform.Find(item.bodyPart.ToString());
            bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = item.mesh;
            bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        }
        /*            foreach (ItemConfig item in defaultConfig.pickedItemAndVariants.Keys)
                    {
                        var bodyTransform = transform.Find(item.bodyPart.ToString());
                        bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + item.bodyPart.ToString()).mesh;
                        bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;

                        *//*Debug.Log("I put " + (ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + item.bodyPart.ToString()) + "ON THE " + item.bodyPart.ToString()));*//*
                    }*/

        if (currentConfig != null)
        {
            foreach (string dirtyPair in currentConfig.pickedItemsAndVariants)
            {
                string[] strs = dirtyPair.Split('+');
                var item = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);
                if (item != null)
                {
                    var bodyTransform = transform.Find(item.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART
                    bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = item.mesh;
                    bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = currentConfig.GetActiveVariant(item).color;
                }
            }
            /*        {
                        foreach (KeyValuePair<ItemConfig, string> pair in currentConfig.pickedItemAndVariants)
                        {
                            {
                                var bodyTransform = transform.Find(pair.Key.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART
                                bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = pair.Key.mesh;
                                bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = Color.white; *//*ScriptableList<ItemVariant>.instance.GetItemByID(pair.Value).color;*//*
                            }
                        }
                    }*/
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, PutOnClothes);
        if (GetComponent<PreviewManager>() != null) //ckeck if this manager is prewiew skin manager
        {
            Messenger.RemoveListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
        }
    }

    void LoadConf()
    {
        string key = _characterSex.ToString() + _gameMode.ToString();
        var json = PlayerPrefs.GetString(key);
        currentConfig = JsonUtility.FromJson<ClothesConfig>(json);

/*        if (currentConfig != null)
        {
            currentConfig.LoadItemsData();
        }
        else
        {
            currentConfig = new ClothesConfig();
        }*/
        if(currentConfig == null)
        {
            currentConfig = new ClothesConfig();
        }
    }

}
public enum Gender
{
    MALE,
    FEMALE
}
