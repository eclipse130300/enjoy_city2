using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPreviewManager : MonoBehaviour
{
    public List<GameObject> roomItems;

    public GameObject GOpreviewing;

    public RoomConfig currentRoomConf;

    public FURNITURE furniturePreviewing;

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
        Messenger.AddListener<RoomItemDisplay>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);
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
        if (currentRoomConf == null)
            currentRoomConf = new RoomConfig();
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
    }

    private void OnItemVariantChanged(ItemVariant variant)
    {
        /* previewingBodyPart.material.color = variant.color;*/
        //HERE WE APPLY VARIANT ON THE MODEL!
        GOpreviewing.GetComponent<MeshRenderer>().material.color = variant.color;
        activeVariant = variant;
    }

    private void OnItemPicked(RoomItemDisplay itemDisplay)
    {
        if (shopManager.CheckIfItemIsBought(itemPreviewing, activeVariant))
        {
            //add item and active variant to config
            currentRoomConf.AddItemToConfig(itemPreviewing, activeVariant);

            //and save it
            SaveRoomConfig();
/*
            Messenger.Broadcast(GameEvents.CLOTHES_CHANGED);*/
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

        foreach(GameObject it in roomItems)
        {
            if (it.name == itemCFG.furnitureType.ToString())
            {
                camMov.target = it.transform;
                GOpreviewing = it;
                it.GetComponent<MeshFilter>().mesh = itemCFG.mesh;
                var itemRenderer = it.GetComponent<MeshRenderer>();
                itemRenderer.material = itemCFG.material;
                itemRenderer.material.color = currentRoomConf?.ItemIsInConfig(itemCFG) == true ?
        currentRoomConf.GetActiveVariant(itemCFG).color : itemCFG.variants[0].color;
            }
        }

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
            OnItemPicked(new RoomItemDisplay());
            Messenger.Broadcast(GameEvents.ROOM_ITEM_BOUGHT, itemPreviewing, activeVariant);
            Messenger.Broadcast(GameEvents.ITEM_OPERATION_DONE);

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
        Messenger.RemoveListener<RoomItemDisplay>(GameEvents.ROOM_ITEM_PICKED, OnItemPicked);
        Messenger.RemoveListener<ItemVariant>(GameEvents.ITEM_VARIANT_CHANGED, OnItemVariantChanged);
    }
}
