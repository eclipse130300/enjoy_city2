﻿using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEditorUiController : MonoBehaviour
{
    [SerializeField] MapConfig playerRoom;
    [SerializeField] GameObject rightPanel;
    [SerializeField] Transform parentForItem;
    [SerializeField] Transform parentForVariants;
    [SerializeField] GameObject variantPrefab;
    [SerializeField] GameObject buyButton;
    [SerializeField] GameObject itemBoughtTab;
    [SerializeField] GameObject doneButton;
    [SerializeField] GameObject variantSlider;
/*    [SerializeField] GameObject giftButton;*/
    [SerializeField] TextMeshProUGUI variantCostText;
    [SerializeField] Image variantCurrencyIMG;
    [SerializeField] TextMeshProUGUI variantNameIDtext;
    [SerializeField] TextMeshProUGUI varDescriptionText;

    [SerializeField] Sprite softCurrencySprite;
    [SerializeField] Sprite hardCurrencySprite;

    public ClothesConfig currentClothesConfig;
    private GameObject itemDisplaying;
    public PreviewManager previewManager;
    private ShopManager shopManager;
    private ItemConfig itemCFG;

    [SerializeField] private Vector2 itemDisplaySize = new Vector2(81, 84);

    private void Awake()
    {
        shopManager = ShopManager.Instance;
        previewManager = FindObjectOfType<PreviewManager>();
        rightPanel.SetActive(false);
        doneButton.SetActive(false);
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, HideItemInfo);
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, HideBuyButton);
        Messenger.AddListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, ManipulateDisplayingInfo);
        Messenger.AddListener<ItemConfig>(GameEvents.ITEM_PICKED, HideDoneButton);
    }

    private void HideDoneButton(ItemConfig arg1)
    {
        doneButton.SetActive(false);
    }

    private void ManipulateDisplayingInfo(ItemVariant var)
    {

        if (shopManager.CheckIfItemIsBought(itemCFG, var)/* || var.cost == 0*/)
        {
            buyButton.SetActive(false);
            itemBoughtTab.SetActive(true);
            // donebutton on
            doneButton.SetActive(true);
/*            Debug.Log("SHOW DONE BUTTON!!!!");*/

        }
        else
        {

            buyButton.SetActive(true);
            itemBoughtTab.SetActive(false);
            //donebutton off
            doneButton.SetActive(false);
        }





        if (SaveManager.Instance.LoadClothesSet(previewManager.GetCurrentKey()).ItemAndVarIsInConfig(itemCFG, var))
        {
            doneButton.SetActive(false);
        }


        variantCostText.text = var.cost.ToString();

        variantCurrencyIMG.sprite = var.currencyType == CurrencyType.HARD ? hardCurrencySprite
            : softCurrencySprite;

        variantNameIDtext.text = var.ConfigId;
        varDescriptionText.text = var.description;
    }



    private void HideBuyButton(ItemConfig cfg, ItemVariant var)  //TODO cfg is unnecessary && var
    {
        //MAY BE DO NOT HIDE?
        buyButton.SetActive(false);
        itemBoughtTab.SetActive(true);
    }

    public void OnItemBuyTap()
    {
        previewManager.TryBuyPreviewingItem();
    }

    private void HideItemInfo()
    {
        /*        Destroy(itemDisplaying);*/
        rightPanel.SetActive(false);
    }

    private void DisplayItem(GameObject itemGO)
    {
        currentClothesConfig = SaveManager.Instance.LoadClothesSet(previewManager.GetCurrentKey());
        variantSlider.SetActive(true); //activate it just in case


        //display item description
        itemCFG = itemGO.GetComponent<ItemDisplay>().itemConfig;
        rightPanel.SetActive(true);
        itemDisplaying = Instantiate(itemGO, rightPanel.transform.position, Quaternion.identity);
        itemDisplaying.transform.SetParent(parentForItem);
        itemDisplaying.GetComponent<RectTransform>().ResetTransform();
        itemDisplaying.GetComponent<RectTransform>().sizeDelta = itemDisplaySize;
        if (shopManager.CheckIfItemIsBought(itemCFG, currentClothesConfig.GetActiveVariant(itemCFG)))
        {
            buyButton.SetActive(false);
        }
        else
        {
            buyButton.SetActive(true);
        }


        //clear all variants
        var children = parentForVariants.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != parentForVariants.transform) Destroy(child.gameObject);
        }


        // spawn variants in RightSlider
        if (itemCFG.variants.Count > 1)
        {
            foreach (ItemVariant V in itemCFG.variants)
            {
                var varGroup = parentForVariants.GetComponent<VariantGroup>();
                var variant = Instantiate(variantPrefab);
                variant.transform.SetParent(parentForVariants);
                var varTab = variant.GetComponent<VariantTab>();
                varTab.group = varGroup;
                varTab.variant = V;
                varTab.group.Subscribe(varTab);
                varTab.tabBackground.color = V.color;
                bool hasActiveVar;
                if (currentClothesConfig.ItemIsInConfig(itemCFG))
                {
                    hasActiveVar = currentClothesConfig.GetActiveVariant(itemCFG) == V ? true : false;
/*                    Debug.Log("Item: " + itemCFG + ", active variant :" + currentClothesConfig.GetActiveVariant(itemCFG));*/
                }
                else
                {
                    hasActiveVar = V == itemCFG.variants[0] ? true : false;
                }
                varTab.activeIMG.gameObject.SetActive(hasActiveVar);

                bool isActive = currentClothesConfig.ItemAndVarIsInConfig(itemCFG, V) == true ? true : false;
                varTab.activeTick.SetActive(isActive);

                bool isBought = shopManager.CheckIfItemIsBought(itemCFG, V)/* || V.cost == 0*/ == true ? false : true;
                varTab.lockIMG.gameObject.SetActive(isBought);
            }
            ManipulateDisplayingInfo(currentClothesConfig.GetActiveVariant(itemCFG));
        }
        else
        {
            variantSlider.SetActive(false);
            ManipulateDisplayingInfo(itemCFG.variants[0]);
        }

        Destroy(itemDisplaying.GetComponent<ItemClick>());
    }

    public void OnDoneButtonTap()
    {
        // add item and variant to config
        previewManager.OnItemPicked();
        //hide right panel?
    }

    public void OnBackButtonTap()
    {
        Loader.Instance.LoadGameScene(playerRoom);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, HideItemInfo);
        Messenger.RemoveListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, HideBuyButton);
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, ManipulateDisplayingInfo);
        Messenger.RemoveListener<ItemConfig>(GameEvents.ITEM_PICKED, HideDoneButton);

    }


}
