using CMS.Config;
using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinsManager : MonoBehaviour
{
    public GameMode _gameMode;
    public Gender _characterSex;
    public ClothesConfig currentConfig;
    public ClothesConfig defaultConfig;

    private void Awake()
    {
        Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
        Messenger.AddListener(GameEvents.CLOTHES_CHANGED, PutOnClothes);
        SetDefaultConfig();
    }

    private void SetDefaultConfig()
    {
        var cfg = new ClothesConfig();

        foreach (string name in Enum.GetNames(typeof(BODY_PART)))
        {
            cfg.pickedItemAndVariants.Add(ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + name), "default");
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

        Debug.Log("I CALL CLOTHES CHANGING METHOD");
   
            foreach (ItemConfig item in defaultConfig.pickedItemAndVariants.Keys)
            {
                var bodyTransform = transform.Find(item.bodyPart.ToString());
                bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + item.bodyPart.ToString()).mesh;
                bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;

                Debug.Log("I put " + (ScriptableList<ItemConfig>.instance.GetItemByID("default" + _characterSex.ToString() + item.bodyPart.ToString()) + "ON THE " + item.bodyPart.ToString()));
            }

        if (currentConfig != null)
        {
            foreach (KeyValuePair<ItemConfig, string> pair in currentConfig.pickedItemAndVariants)
            {
                {
                    var bodyTransform = transform.Find(pair.Key.bodyPart.ToString()); //IF YOU WANT RENAME 3DMODEL PARTS - RENAME ENUM BODY_PART
                    bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = pair.Key.mesh;
                    bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = ScriptableList<ItemVariant>.instance.GetItemByID(pair.Value).color;
                }
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
public enum Gender
{
    MALE,
    FEMALE
}
