﻿using CMS.Config;
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

    private ShopManager shopManager;

    private void Awake()
    {
        shopManager = ShopManager.Instance;

        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.AddListener<ItemDisplay>(GameEvents.ITEM_PICKED, OnItemPicked);
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

        string key = previewingCharSex.ToString() + previewingGameMode.ToString();
        previewingClothesConfig = SaveManager.Instance.LoadClothesSet(key);
    }

    private void OnItemVariantChanged(ItemVariant variant)
    {
        previewingBodyPart.material.color = variant.color;
        activeVariant = variant;
    }

    private void OnItemPicked(ItemDisplay iDisplay)
    {
        if (shopManager.CheckIfItemIsBought(itemPreviewing, activeVariant))
        {
            //add item and active variant to config
            previewingClothesConfig.AddItemToConfig(itemPreviewing, activeVariant);

            //and save it
            SavePreviewingConfig();

            Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);
        }
        else
        {
            // make a window - BUY ITEM, BRO
            Debug.Log("BUY THIS ITEM FIRST!");
        }
    }

    void SavePreviewingConfig()
    {
        string key = previewingCharSex.ToString() + previewingGameMode.ToString();
        SaveManager.Instance.SaveClothesSet(key, previewingClothesConfig); 
    }
    
    //show item at model(preview)
    private void OnItemPressed(GameObject item)
    {
    
        var itemCFG = item.GetComponent<ItemDisplay>().itemConfig;
        activeVariant = previewingClothesConfig?.GetActiveVariant(itemCFG);
    
        var bodyPart = transform.Find(itemCFG.bodyPart.ToString());
        previewingBodyPart = bodyPart.GetComponent<SkinnedMeshRenderer>();
        previewingBodyPart.sharedMesh = itemCFG.mesh;
    
    previewingBodyPart.material.color = previewingClothesConfig.ItemIsInConfig(itemCFG) == true ?
    previewingClothesConfig.GetActiveVariant(itemCFG).color : /*Color.white*/ itemCFG.variants[0].color;
    
        itemPreviewing = itemCFG;
    }

    public void TryBuyPreviewingItem()
    {
        if (shopManager.CheckIsEnoughMoney(activeVariant.currencyType, activeVariant.cost))
        {
            shopManager.Buy(itemPreviewing, activeVariant, activeVariant.cost, activeVariant.currencyType);
            Debug.Log("I buy: " + itemPreviewing.ConfigId + " in variant: " + activeVariant.ConfigId);
            OnItemPicked(new ItemDisplay());
            Messenger.Broadcast(GameEvents.ITEM_BOUGHT, itemPreviewing, activeVariant);
            Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);
        }
        else
        {
            Debug.Log("nope...ADD MONEY FIRST!");
        }
    }
    
    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.RemoveListener<ItemDisplay>(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
    }

}