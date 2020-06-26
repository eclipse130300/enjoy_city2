using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomPreviewManager : MonoBehaviour
{
    public List<GameObject> roomItems;

    public FURNITURE furniturePreviewing;

    public List<GameObject> ObjectsPreviewing;

    public RoomConfig currentRoomConf;


    public SaveManager saveManager;

    public ShopManager shopManager;

    public ItemVariant activeVariant;

    public RoomItemConfig itemPreviewing;

    public Camera previewCamera;

    private RoomCameraMover camMov;

    private void Awake()
    {
        saveManager = SaveManager.Instance;
        shopManager = ShopManager.Instance;
        camMov = previewCamera.GetComponent<RoomCameraMover>();

        Messenger.AddListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, OnFurnitureChanged);
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
/*        Messenger.AddListener<RoomItemDisplay>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);*/
        Messenger.AddListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
        
    }

    void OnFurnitureChanged(FURNITURE funit)
    {
        foreach (GameObject it in roomItems)
        {
            if (it.name == funit.ToString())
            {
                camMov.target = it.transform;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void LoadRoomConfig()
    {
        currentRoomConf = saveManager.LoadRoomSet();
/*        if (currentRoomConf == null)
            currentRoomConf = new RoomConfig();*/
    }
    void SaveRoomConfig()
    {
        saveManager.SaveRoomSet(currentRoomConf);
    }

    void Initialize()
    {
        foreach (string name in Enum.GetNames(typeof(FURNITURE)))
        {
            var privewingItems = FindObjectsOfType<IPreviewable>(); //just tag previewing gameobjects with this
            foreach (var item in privewingItems)
            {
                if (item.gameObject.name == name) roomItems.Add(item.gameObject);
            }
        }

        LoadRoomConfig();
        TryAddDefaultItems();
    }

    private void TryAddDefaultItems() //first add default items - they should be opened instantly
    {
/*        LoadRoomConfig();*/

        if (currentRoomConf.pickedItemsAndVariants.Count == 0)
        {

            var allDefaultItems = ScriptableList<RoomItemConfig>.instance.list.Where(t => t.isDefault).ToList();

            foreach (RoomItemConfig defaultItem in allDefaultItems)
            {
                currentRoomConf.AddItemToConfig(defaultItem, defaultItem.variants?[0]);
                shopManager.Buy(defaultItem, defaultItem.variants?[0], 0, CurrencyType.SOFT);
            }
            SaveRoomConfig();
        }
    }

    private void OnItemVariantChanged(ItemVariant variant)
    {
        foreach (var Gobject in ObjectsPreviewing)
        {
            //HERE WE APPLY VARIANT ON THE MODEL!
            Gobject.GetComponent<MeshRenderer>().material.color = variant.color;
        }
        activeVariant = variant;
    }

/*    public void OnItemPicked(RoomItemDisplay itemDisplay)
    {
        if (shopManager.CheckIfItemIsBought(itemPreviewing, activeVariant))
        {
            //add item and active variant to config
            currentRoomConf.AddItemToConfig(itemPreviewing, activeVariant);

            //and save it
            SaveRoomConfig();

*//*            Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);*//*
        }
        else
        {
            // make a window - BUY ITEM, BRO
            Debug.Log("BUY THIS ITEM FIRST!");
        }
    }*/

    public void OnItemPicked()
    {
        if (shopManager.CheckIfItemIsBought(itemPreviewing, activeVariant))
        {
            //add item and active variant to config
            currentRoomConf.AddItemToConfig(itemPreviewing, activeVariant);

            //and save it
            SaveRoomConfig();

            Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);
            Messenger.Broadcast(GameEvents.ROOM_ITEM_PICKED, itemPreviewing);
/*            Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE); //todo it's the same logically as Clothes_changed event?*/
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
        ObjectsPreviewing.Clear();
        var itemCFG = item.GetComponent<RoomItemDisplay>().itemConfig;
        activeVariant = currentRoomConf?.GetActiveVariant(itemCFG);

        foreach(GameObject it in roomItems)
        {
            if (it.name == itemCFG.furnitureType.ToString())
            {
                camMov.target = it.transform;
                ObjectsPreviewing.Add(it);
                if (itemCFG.mesh != null)
                {
                    it.GetComponent<MeshFilter>().mesh = itemCFG.mesh;
                }

                var itemRenderer = it.GetComponent<MeshRenderer>();
                itemRenderer.material = itemCFG.material;
                itemRenderer.material.color = currentRoomConf?.ItemIsInConfig(itemCFG) == true ?
        currentRoomConf.GetActiveVariant(itemCFG).color : itemCFG.variants[0].color;
            }
        }
        itemPreviewing = itemCFG;
    }

    public void TryBuyPreviewingItem()
    {
        if (shopManager.CheckIsEnoughMoney(activeVariant.currencyType, activeVariant.cost))
        {
            shopManager.Buy(itemPreviewing, activeVariant, activeVariant.cost, activeVariant.currencyType);
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
        Messenger.RemoveListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, OnFurnitureChanged);
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, OnItemPressed);
/*        Messenger.RemoveListener<RoomItemDisplay>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);*/
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
    }
}
