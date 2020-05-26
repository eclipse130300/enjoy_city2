using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PreviewManager : MonoBehaviour
{
    public Gender previewingCharSex;

    public GameMode previewingGameMode;

    private SkinnedMeshRenderer previewingBodyPart;

    public ClothesConfig previewingClothesConfig;

    public ClothesConfig defaultConfig;

    public ItemConfig itemPreviewing;

    public ItemVariant activeVariant;

    private void Awake()
    {
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.AddListener(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.AddListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged); //texture as well
        Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);

        LoadConf();
    }

    private void Start()
    {
        Messenger.Broadcast(GameEvents.GENDER_CHANGED, previewingCharSex); // TODO MAKE GENDER(character) MANAGER?
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        previewingGameMode = gameMode;
        LoadConf();
    }

    void LoadConf()
    {
        /*PlayerPrefs.DeleteAll();*/


        string key = previewingCharSex.ToString() + previewingGameMode.ToString();
        var json = PlayerPrefs.GetString(key);
        previewingClothesConfig = JsonUtility.FromJson<ClothesConfig>(json);

        if (previewingClothesConfig != null)
        {
            previewingClothesConfig.LoadItemsData();
        }
        else
        {
            previewingClothesConfig = new ClothesConfig();
        }
    }

    private void OnItemVariantChanged(ItemVariant variant)
    {
        previewingBodyPart.material.color = variant.color;
        activeVariant = variant;
    }

    private void OnItemPicked()
    {
        //add item and active variant to config
        if (activeVariant == null)
        {
                previewingClothesConfig.AdditemToConfig(itemPreviewing, itemPreviewing.variants[0].ConfigId);
        }
        else
        {
            previewingClothesConfig.AdditemToConfig(itemPreviewing, activeVariant.ConfigId);
        }


        //and save it
        SavePreviewingConfig();

        Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);
    }

    void SavePreviewingConfig()
    {

        string key = previewingCharSex.ToString() + previewingGameMode.ToString();
        var json = JsonUtility.ToJson(previewingClothesConfig);
        PlayerPrefs.SetString(key, json);
    }

        //clears items from model
    private void OnItemAbort()
    {
        Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);
        itemPreviewing = null;
        activeVariant = null;
    }
    
    //show item at model(preview)
    private void OnItemPressed(GameObject item)
    {
/*        OnItemAbort();*/
    
        var itemCFG = item.GetComponent<ItemDisplay>().itemConfig;
        activeVariant = previewingClothesConfig.GetActiveVariant(itemCFG);
    
        var bodyPart = transform.Find(itemCFG.bodyPart.ToString());
        previewingBodyPart = bodyPart.GetComponent<SkinnedMeshRenderer>();
        previewingBodyPart.sharedMesh = itemCFG.mesh;
    
    previewingBodyPart.material.color = previewingClothesConfig.ItemIsInConfig(itemCFG) == true ?
    previewingClothesConfig.GetActiveVariant(itemCFG).color : /*Color.white*/ itemCFG.variants[0].color;
    
        itemPreviewing = itemCFG;
    }
    
    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.RemoveListener(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
    }

}