using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomInventoryManager : MonoBehaviour
{

    public List<RoomItemConfig> inventory; //
    [SerializeField] private int columnsCount;
    [SerializeField] private int inventoryMinSize;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private GameObject EmptySlot;
    [SerializeField] private Transform contentObject;

    ScriptableList<RoomItemConfig> SLinstance;
    /*private IInventoryDisplayer<BaseScriptableDrowableItem> itemCFG;*/
    /*private IInventoryDisplayer<RoomItemConfig> CFG;*/

    private ShopManager shopManager;

    public FURNITURE furniture_type;


    private void Awake()
    {
        shopManager = ShopManager.Instance;
        SLinstance = ScriptableList<RoomItemConfig>.instance;

        Messenger.AddListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, OnFurnitureChanged);
    }


    private void Start()
    {
        OnFurnitureChanged(FURNITURE.SOFA);
    }


    public void OnFurnitureChanged(FURNITURE furniture)
    {
        furniture_type = furniture;

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
        foreach (RoomItemConfig cfg in inventory) //insantiate in inv
        {
            var item = Instantiate(ItemPrefab);
            var itemScript = item.GetComponent<RoomItemDisplay>();
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
        //sort for room items
        inventory = SLinstance.list.
           Where(t => t.furnitureType == furniture_type).ToList();

    }



    private void OnDestroy()
    {
        Messenger.RemoveListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, OnFurnitureChanged);
    }
}

public enum FURNITURE
{
    SOFA,
    PICTURE,
    TABLE
}
