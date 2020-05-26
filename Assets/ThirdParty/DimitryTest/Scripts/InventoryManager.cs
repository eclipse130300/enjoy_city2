using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    List<ItemConfig> inventory;
    [SerializeField] private int columnsCount;
    [SerializeField] private int inventoryMinSize;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private GameObject EmptySlot;
    [SerializeField] private Transform contentObject;

    public GameMode currentMode;
    public BODY_PART currentbodyPart;
    public Gender characterGender;

    private void Awake()
    { 
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
        ClearInventory();
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

    private void DisplayAppropriateItems()
    {
        foreach (ItemConfig cfg in inventory)
        {
            var item = Instantiate(ItemPrefab);
            var itemScript = item.GetComponent<ItemDisplay>();
            item.transform.SetParent(contentObject);

            itemScript.itemConfig = cfg;
            itemScript.SetItem(cfg.Inventory_image, cfg.Inventory_frameColor);
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

    private void GetItems()
    {
        inventory = ScriptableList<ItemConfig>.instance.list.
            Where(t => t.bodyPart == currentbodyPart).
            Where(t => t.gameMode == currentMode).
            Where(t => t.gender == characterGender).
            Where(t => !t.ToString().Contains("default")).
            ToList();

        
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameMode>(GameEvents.INVENTORY_GAME_MODE_CHANGED, GameModeChanged);
        Messenger.RemoveListener<BODY_PART>(GameEvents.INVENTORY_BODY_PART_CHANGED, BodyPartChanged);
        Messenger.AddListener<Gender>(GameEvents.GENDER_CHANGED, OnGenderChanged);
    }
}
