﻿using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class PreviewManager : MonoBehaviour
{
    public static Gender previewingCharSex;

    public static GameMode previewingGameMode;

/*    private SkinnedMeshRenderer previewingBodyPart;*/

    public ClothesConfig previewingClothesConfig;

    public ClothesConfig defaultConfig;

    public ItemConfig itemPreviewing;

    public ItemVariant activeVariant;

    private ShopManager shopManager;

    private SkinsManager skinsManager;


    private void Awake()
    {
        shopManager = ShopManager.Instance;
        skinsManager = GetComponent<SkinsManager>();
        previewingCharSex = SaveManager.Instance.LoadBody().gender;

        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
/*        Messenger.AddListener<ItemDisplay>(GameEvents.ITEM_PICKED, OnItemPicked);*/
        Messenger.AddListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged); //texture as well
        Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);

/*        TryBuyAllDefaultItems(); //in case it is our 1rst launch, lets buy all default items!

        OnGameModeChanged(GameMode.SandBox);*/
    }

    private void Start()
    {
        TryBuyAllDefaultItems(); //in case it is our 1rst launch, lets buy all default items!

        OnGameModeChanged(GameMode.SandBox);
    }

    private void TryBuyAllDefaultItems()
    {
        var allGameModes = Enum.GetValues(typeof(GameMode));
        foreach (GameMode mode in allGameModes)
        {
            previewingGameMode = mode;
            LoadConf();
            TryAddDefaultItems();
        }
    }

    public string GetCurrentKey()
    {
        /*return previewingCharSex.ToString() + previewingGameMode.ToString();*/
/*        Debug.Log(previewingCharSex.ToString() + previewingGameMode.ToString());*/
        return SaveManager.Instance.LoadBody().gender + previewingGameMode.ToString();
    }

    private void TryAddDefaultItems() //first add default items - they should be opened instantly
    {
        if (previewingClothesConfig.pickedItemsAndVariants.Count != 0) return;

        var allDefaultItems = ScriptableList<ItemConfig>.instance.list.Where(t => t.isDefault).Where(t => t.gender == previewingCharSex).Where(t => t.gameMode == previewingGameMode).ToList();

        foreach (ItemConfig defaultItem in allDefaultItems)
        {
            previewingClothesConfig.AddItemToConfig(defaultItem, defaultItem.variants?[0]);
            shopManager.Buy(defaultItem, defaultItem.variants?[0], 0, CurrencyType.SOFT);
        }
        SavePreviewingConfig();

/*        Debug.Log("Default items added!");*/

        //LoadConf();
    }

    private void OnGameModeChanged(GameMode gameMode)
    {
        previewingGameMode = gameMode;
        LoadConf();
        TryAddDefaultItems();

        skinsManager.PutOnClothes(previewingClothesConfig);
    }

    void LoadConf()
    {
        string key = GetCurrentKey();/*previewingCharSex.ToString() + previewingGameMode.ToString();*/
        previewingClothesConfig = SaveManager.Instance.LoadClothesSet(key);
    }

    private void OnItemVariantChanged(ItemVariant variant)
    {

        GetComponent<SkinsManager>().PutOnItem(itemPreviewing, variant);
        //previewingBodyPart.material.color = variant.color;
        activeVariant = variant;
    }

    public void OnItemPicked()
    {
        if (shopManager.CheckIfItemIsBought(itemPreviewing, activeVariant))
        {
            //add item and active variant to config
            previewingClothesConfig.AddItemToConfig(itemPreviewing, activeVariant);

            //and save it
            SavePreviewingConfig();

            Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);
            Messenger.Broadcast(GameEvents.ITEM_PICKED, itemPreviewing);
/*            Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE); //todo it's the same logically as Clothes_changed event?*/
        }
        else
        {
            // make a window - BUY ITEM, BRO
            Debug.Log("BUY THIS ITEM FIRST!");
        }
    }

    void SavePreviewingConfig()
    {
        string key = GetCurrentKey(); /*previewingCharSex.ToString() + previewingGameMode.ToString();*/
/*        Debug.Log(GetCurrentKey().ToString());*/
        SaveManager.Instance.SaveClothesSet(key, previewingClothesConfig);
        SaveManager.Instance.SaveShopConfig();
        SaveManager.Instance.SaveChangableConfig();
    }
    
    //show item at model(preview)
    private void OnItemPressed(GameObject item)
    {
        var itemCFG = item.GetComponent<ItemDisplay>().itemConfig;
        activeVariant = previewingClothesConfig?.GetActiveVariant(itemCFG);

        GetComponent<SkinsManager>().PutOnItem(itemCFG, activeVariant);

        /*
        var bodyPart = transform.Find(itemCFG.bodyPart.ToString());
        previewingBodyPart = bodyPart.GetComponent<SkinnedMeshRenderer>();
        previewingBodyPart.sharedMesh = itemCFG.mesh;
    
        previewingBodyPart.material.color = previewingClothesConfig.ItemIsInConfig(itemCFG) == true ?
        previewingClothesConfig.GetActiveVariant(itemCFG).color :  itemCFG.variants[0].color;
    */
        itemPreviewing = itemCFG;
    }

    public void TryBuyPreviewingItem()
    {
        if (shopManager.CheckIsEnoughMoney(activeVariant.currencyType, activeVariant.cost))
        {
            shopManager.Buy(itemPreviewing, activeVariant, activeVariant.cost, activeVariant.currencyType);
            Debug.Log("I buy: " + itemPreviewing.ConfigId + " in variant: " + activeVariant.ConfigId);
            OnItemPicked();
            Messenger.Broadcast(GameEvents.ITEM_BOUGHT, itemPreviewing, activeVariant);
/*            Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);*/
        }
        else
        {
            Debug.Log("nope...ADD MONEY FIRST!");
        }
    }
    
    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
/*        Messenger.RemoveListener<ItemDisplay>(GameEvents.ITEM_PICKED, OnItemPicked);*/
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
        Messenger.RemoveListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, OnGameModeChanged);
    }

}