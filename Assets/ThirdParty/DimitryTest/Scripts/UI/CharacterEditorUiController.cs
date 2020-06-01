using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.XR;
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
    [SerializeField] TextMeshProUGUI variantCostText;
    [SerializeField] Image variantCurrencyIMG;
    [SerializeField] TextMeshProUGUI variantNameIDtext;
    [SerializeField] TextMeshProUGUI varDescriptionText;

    [SerializeField] Sprite softCurrencySprite;
    [SerializeField] Sprite hardCurrencySprite;

    public ClothesConfig currentClothesConfig;
    private GameObject itemDisplaying;
    private PreviewManager previewManager;
    private ShopManager shopManager;
    private ItemConfig itemCFG;

    [SerializeField] private Vector2 itemDisplaySize = new Vector2(81, 84); 

    private void Awake()
    {
        shopManager = ShopManager.Instance;
        previewManager = FindObjectOfType<PreviewManager>();
        rightPanel.SetActive(false);
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, HideItemInfo);
        Messenger.AddListener<ClothesConfig>(GameEvents.CLOTHES_CONFIG_LOADED, SetCurrentClothesConfig);
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, HideBuyButton);

        Messenger.AddListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, ManipulateDisplayingInfo);
    }

    private void ManipulateDisplayingInfo(ItemVariant var)
    {
        if (shopManager.CheckIfItemIsBought(itemCFG, var))
        {
            buyButton.SetActive(false);
            itemBoughtTab.SetActive(true);
        }
        else
        {
            buyButton.SetActive(true);
            itemBoughtTab.SetActive(false);
        }

        variantCostText.text = var.cost.ToString();

        variantCurrencyIMG.sprite = var.currencyType == CurrencyType.HARD ?  hardCurrencySprite
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

    private void SetCurrentClothesConfig(ClothesConfig cfg)
    {
        currentClothesConfig = cfg;
    }

    private void HideItemInfo()
    {
        Destroy(itemDisplaying);
        rightPanel.SetActive(false);
    }

    private void DisplayItem(GameObject itemGO) 
    {
        //display item description
        itemCFG = itemGO.GetComponent<ItemDisplay>().itemConfig;
        rightPanel.SetActive(true);
        itemDisplaying = Instantiate(itemGO, rightPanel.transform.position,  Quaternion.identity);
        itemDisplaying.transform.SetParent(parentForItem);
        itemDisplaying.GetComponent<RectTransform>().ResetTransform();
        itemDisplaying.GetComponent<RectTransform>().sizeDelta = itemDisplaySize;
        if(shopManager.CheckIfItemIsBought(itemCFG, currentClothesConfig.GetActiveVariant(itemCFG)))
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
            bool val;
            if (currentClothesConfig.ItemIsInConfig(itemCFG))
            {
                val = currentClothesConfig.GetActiveVariant(itemCFG) == V ? true : false;
/*                Debug.Log("Item: " + itemCFG + ", active variant :" + currentClothesConfig.GetActiveVariant(itemCFG));*/
            }
            else
            {
                val = V == itemCFG.variants[0] ? true : false;
            }
            varTab.activeIMG.gameObject.SetActive(val);

            bool value = shopManager.CheckIfItemIsBought(itemCFG, V) == true ? false : true;
            varTab.lockIMG.gameObject.SetActive(value);
        }


        ManipulateDisplayingInfo(currentClothesConfig.GetActiveVariant(itemCFG));
        Destroy(itemDisplaying.GetComponent<ItemClick>());
    }

    public void OnDoneButtonTap()
    {
        Loader.Instance.LoadGameScene(playerRoom);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, HideItemInfo);

        Messenger.RemoveListener<ClothesConfig>(GameEvents.CLOTHES_CONFIG_LOADED, SetCurrentClothesConfig);
        Messenger.RemoveListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, HideBuyButton);

    }


}
