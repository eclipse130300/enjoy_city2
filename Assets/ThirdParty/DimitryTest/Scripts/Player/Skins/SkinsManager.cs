using CMS.Config;
using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinsManager : MonoBehaviour
{
    public GameMode _gameMode;
    public Sex _characterSex;
    public ClothesConfig currentConfig;

    private void Awake()
    {
        Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
        Messenger.AddListener(GameEvents.CLOTHES_CHANGED, PutOnClothes);
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

        if (currentConfig != null)
        {
            foreach (KeyValuePair<ItemConfig, string> pair in currentConfig.pickedItemAndVariants)
            {
                var bodyTransform = transform.Find(pair.Key.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART
                bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = pair.Key.mesh;
                bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = ScriptableList<ItemVariant>.instance.GetItemByID(pair.Value).color;

                //mistake!
            }
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvents.CLOTHES_CHANGED, PutOnClothes);
        Messenger.RemoveListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
    }

    void LoadConf()
    {
        string key = _characterSex.ToString() + _gameMode.ToString();
        var json = PlayerPrefs.GetString(key);
        currentConfig = JsonUtility.FromJson<ClothesConfig>(json);

        currentConfig?.LoadItemsData();

    }
}
public enum Sex
{
    Male,
    Female
}
