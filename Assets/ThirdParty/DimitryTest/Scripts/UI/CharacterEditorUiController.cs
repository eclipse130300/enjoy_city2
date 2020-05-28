using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditorUiController : MonoBehaviour
{
    [SerializeField] MapConfig playerRoom;
    [SerializeField] GameObject rightPanel;
    [SerializeField] Transform parentForItem;
    [SerializeField] Transform parentForVariants;
    [SerializeField] GameObject variantPrefab;

    public ClothesConfig currentClothesConfig;
    private GameObject itemDisplaying;
    private PreviewManager previewManager;


    [SerializeField] private Vector2 itemDisplaySize = new Vector2(81, 84); 

    private void Awake()
    {
        previewManager = FindObjectOfType<PreviewManager>();
        rightPanel.SetActive(false);
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.AddListener(GameEvents.ITEM_OPERATION_DONE, HideItemInfo);
        Messenger.AddListener<ClothesConfig>(GameEvents.CLOTHES_CONFIG_LOADED, SetCurrentClothesConfig);
        /*Messenger.AddListener(GameEvents.ITEM_BOUGHT, OnItemBought);*/
    }

    private void OnItemBuyTap()
    {
        //try to buy 

        //hide buy button
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
        var itemCFG = itemGO.GetComponent<ItemDisplay>().itemConfig;
        rightPanel.SetActive(true);
        itemDisplaying = Instantiate(itemGO, rightPanel.transform.position,  Quaternion.identity);
        itemDisplaying.transform.SetParent(parentForItem);
        itemDisplaying.GetComponent<RectTransform>().ResetTransform();
        itemDisplaying.GetComponent<RectTransform>().sizeDelta = itemDisplaySize;

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
            float alpha = 0f;
            if (currentClothesConfig.ItemIsInConfig(itemCFG))
            {
                alpha = currentClothesConfig.GetActiveVariant(itemCFG) == V ? 1 : 0;
                varGroup.SetTabFrameAlpha(alpha, varTab);
                Debug.Log("Item: " + itemCFG + ", active variant :" + currentClothesConfig.GetActiveVariant(itemCFG));
            }
            else
            {
                alpha = V == itemCFG.variants[0] ? 1 : 0;
                varGroup.SetTabFrameAlpha(alpha, varTab);
            }

        }

        Destroy(itemDisplaying.GetComponent<ItemDisplay>());
    }

    public void OnDoneButtonTap()
    {
        Loader.Instance.LoadGameScene(playerRoom);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.RemoveListener(GameEvents.ITEM_OPERATION_DONE, HideItemInfo);
    }


}
