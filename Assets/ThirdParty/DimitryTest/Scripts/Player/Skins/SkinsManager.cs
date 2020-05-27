using CMS.Config;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// set as a component of parent of modelBodyparts
public class SkinsManager : MonoBehaviour //TODO MAKE UPDATE DEPENDING ON THE GENDER
{
    public GameMode _gameMode;
    public Gender _characterSex;
    public ClothesConfig currentConfig;
    public ClothesConfig defaultConfig;


    private void Awake()
    {

        if (GetComponent<PreviewManager>() != null) //ckeck if we are in character editor
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


    private void Start()
    {
        /*PlayerPrefs.DeleteAll();*/

        PutOnClothes();

    }

    // puts on real model
    private void PutOnClothes()
    {
        LoadConf();
        //PUT ON DEFAULT CLOTHES FIRST
        foreach (string dirtyPair in defaultConfig.pickedItemsAndVariants)
        {
            string[] strs = dirtyPair.Split('+');
            var item = ScriptableList<ItemConfig>.instance.GetItemByID(strs[0]);
            var bodyTransform = transform.Find(item.bodyPart.ToString());
            bodyTransform.GetComponent<SkinnedMeshRenderer>().sharedMesh = item.mesh;
            bodyTransform.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        }
        //PUT ON CLOTHES FROM CONFIG
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
        currentConfig = SaveManager.Instance.LoadClothesSet(key);

        Messenger.Broadcast(GameEvents.CLOTHES_CONFIG_LOADED, currentConfig); //!!
    }

}
public enum Gender
{
    MALE,
    FEMALE
}
