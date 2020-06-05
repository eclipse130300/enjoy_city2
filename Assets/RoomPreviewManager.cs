using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPreviewManager : MonoBehaviour
{
    public List<GameObject> roomItems;

    public RoomConfig currentRoomConf;

    public FURNITURE furniturePreviewing;

    public SaveManager saveManager;

    public ShopManager shopManager;

    public ItemVariant activeVariant;

    public RoomItemConfig itemPreviewing;

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        shopManager = ShopManager.Instance;

        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.AddListener(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.AddListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void LoadRoomConfig()
    {
        currentRoomConf = saveManager.LoadRoomSet();
    }
    void SaveRoomConfig()
    {
        saveManager.SaveRoomSet(currentRoomConf);
    }

    void Initialize()
    {
        foreach (string name in Enum.GetNames(typeof(FURNITURE)))
        {
            roomItems.Add(GameObject.Find(name));
        }

        LoadRoomConfig();
    }

    private void OnItemVariantChanged(ItemVariant variant)
    {
       /* previewingBodyPart.material.color = variant.color;*/
        activeVariant = variant;
    }

    private void OnItemPicked()
    {
        if (shopManager.CheckIfItemIsBought(itemPreviewing, activeVariant))
        {
            //add item and active variant to config
            currentRoomConf.AddItemToConfig(itemPreviewing, activeVariant);

            //and save it
            SaveRoomConfig();

            Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);
        }
        else
        {
            // make a window - BUY ITEM, BRO
            Debug.Log("BUY THIS ITEM FIRST!");
        }
    }

    //show item at model(preview)
    private void OnItemPressed(GameObject item)
    {

        var itemCFG = item.GetComponent<RoomItemDisplay>().itemConfig;
        activeVariant = currentRoomConf?.GetActiveVariant(itemCFG);

/*        var bodyPart = transform.Find(itemCFG.bodyPart.ToString());
        previewingBodyPart = bodyPart.GetComponent<SkinnedMeshRenderer>();
        previewingBodyPart.sharedMesh = itemCFG.mesh;

        previewingBodyPart.material.color = currentRoomConf.ItemIsInConfig(itemCFG) == true ?
        currentRoomConf.GetActiveVariant(itemCFG).color : *//*Color.white*//* itemCFG.variants[0].color;*/

        itemPreviewing = itemCFG;
    }

    public void TryBuyPreviewingItem()
    {
        if (shopManager.CheckIsEnoughMoney(activeVariant.currencyType, activeVariant.cost))
        {
            shopManager.Buy(itemPreviewing, activeVariant, activeVariant.cost, activeVariant.currencyType);
            Debug.Log("I buy: " + itemPreviewing.ConfigId + " in variant: " + activeVariant.ConfigId);
            OnItemPicked();
            Messenger.Broadcast(GameEvents.ROOM_ITEM_BOUGHT, itemPreviewing, activeVariant);
        }
        else
        {
            Debug.Log("nope...ADD MONEY FIRST!");
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
        Messenger.RemoveListener(GameEvents.ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
    }
}
