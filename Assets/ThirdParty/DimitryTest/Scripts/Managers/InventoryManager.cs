using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public INVENTORY_TYPE invType;

    List<ItemConfig> inventory; //
    [SerializeField] private int columnsCount;
    [SerializeField] private int inventoryMinSize;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private GameObject EmptySlot;
    [SerializeField] private Transform contentObject;

    /*private IInventoryDisplayer<BaseScriptableDrowableItem> itemCFG;*/
    /*private IInventoryDisplayer<RoomItemConfig> CFG;*/

    private ShopManager shopManager;

    public GameMode currentMode;
    public BODY_PART currentbodyPart;
    public Gender characterGender;


    private void Awake()
    {
        shopManager = ShopManager.Instance;

        /*itemCFG = GetComponent<IInventoryDisplayer<BaseScriptableDrowableItem>>();*/

        Messenger.AddListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, GameModeChanged);
        Messenger.AddListener<BODY_PART>(GameEvents.INVENTORY_BODY_PART_CHANGED, BodyPartChanged);
        Messenger.AddListener<Gender>(GameEvents.GENDER_CHANGED, OnGenderChanged);
    }

    private void OnGenderChanged(Gender gender)
    {
        characterGender = gender;
    }

    private void Start()
    {
        OnInventoryChanged(GameMode.SandBox, BODY_PART.HAIR);
    }


    public void OnInventoryChanged(GameMode mode, BODY_PART part)
    {
        currentMode = mode;
        currentbodyPart = part;

        RefreshInventory();
    }

    public void GameModeChanged(GameMode mode)
    {
        currentMode = mode;
        RefreshInventory();
    }

    public void BodyPartChanged(BODY_PART part)
    {
        currentbodyPart = part;
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        ClearInventory();
        GetItems(); 
        DisplayAppropriateItems();
    }

    private void ClearInventory()
    {
        var children = contentObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != this.transform) Destroy(child.gameObject);
        }
    }

    private void DisplayAppropriateItems() //here is a diff
    {
        foreach (ItemConfig cfg in  inventory) //insantiate in inv
        {
            var item = Instantiate(ItemPrefab);
            var itemScript = item.GetComponent<ItemDisplay>();
            item.transform.SetParent(contentObject);


            itemScript.itemConfig = cfg; //initialize
            itemScript.SetItem(cfg.Inventory_image, cfg.Inventory_frameColor);
            bool lockVal = shopManager.CheckIfItemIsBought(cfg) == true ? false : true;
            itemScript.lockIcon.gameObject.SetActive(lockVal);
        }
        if (inventory?.Count < inventoryMinSize)
        {
            int emptySlots = inventoryMinSize - inventory.Count;
            for (int i = 0; i < emptySlots; i++)
            {
                InstantiateEmptyItem();
            }
        }
        else if (inventory?.Count % columnsCount != 0)
        {
            int slotsToAdd = inventory.Count % columnsCount;

            for (int i = 0; i < slotsToAdd; i++)
            {
                InstantiateEmptyItem();
            }
        }
    }

    private void InstantiateEmptyItem()
    {
        var emptyItem = Instantiate(EmptySlot);
        emptyItem.transform.SetParent(contentObject);
    }

    private void GetItems() //here is a diff
    {
        switch (invType)
        {
            case INVENTORY_TYPE.CHARACTER_3D:
                //sort for 3d char items
                inventory = ScriptableList<ItemConfig>.instance.list.
                Where(t => t.bodyPart == currentbodyPart).
                Where(t => t.gameMode == currentMode).
                Where(t => t.gender == characterGender).
                Where(t => !t.ToString().Contains("default")).
                ToList();
                break;
            case INVENTORY_TYPE.ROOM:
                //sort for roomitems

                break;      
        }
        
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, GameModeChanged);
        Messenger.RemoveListener<BODY_PART>(GameEvents.INVENTORY_BODY_PART_CHANGED, BodyPartChanged);
        Messenger.AddListener<Gender>(GameEvents.GENDER_CHANGED, OnGenderChanged);
    }

}
    public enum INVENTORY_TYPE 
    {
        CHARACTER_3D,
            ROOM,
    }
