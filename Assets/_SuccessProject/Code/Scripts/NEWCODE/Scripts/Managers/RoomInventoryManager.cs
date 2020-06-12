using CMS.Config;
using System.Collections.Generic;
using System.Linq;

public class RoomInventoryManager : BaseInventoryManager
{

    public List<RoomItemConfig> inventory; //
    public ScriptableList<RoomItemConfig> SLinstance;


    public FURNITURE furniture_type;


    protected override void Awake()
    {
        base.Awake();

        SLinstance = ScriptableList<RoomItemConfig>.instance;
        Messenger.AddListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, InitializeInventory);
    }


    private void Start()
    {
        InitializeInventory(FURNITURE.SOFA);
    }


    public void InitializeInventory(FURNITURE furniture)
    {
        furniture_type = furniture;

        RefreshInventory();
    }

    protected override void DisplayAppropriateItems() //here is a diff
    {
        foreach (RoomItemConfig cfg in inventory) //insantiate in inv
        {
            var item = Instantiate(ItemPrefab);
            var itemScript = item.GetComponent<RoomItemDisplay>();
            item.transform.SetParent(contentObject);


            itemScript.itemConfig = cfg; //initialize
            itemScript.SetItem(cfg.Inventory_image, cfg.Inventory_frameColor, CheckIfItemIsActive(cfg));
            bool lockVal = shopManager.CheckIfItemIsBought(cfg)/* || SaveManager.Instance.GetActiveVariant(cfg).cost == 0 */== true ? false : true; //wtf!!!221
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

    private bool CheckIfItemIsActive(RoomItemConfig cfg)
    {
        RoomConfig activeClothes = SaveManager.Instance.LoadRoomSet();
        if (activeClothes == null) return false;

        if (activeClothes.ItemIsInConfig(cfg))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void GetItems() //here is a diff
    {
        //sort for room items
        inventory = SLinstance.list.
           Where(t => t.furnitureType == furniture_type).ToList();

    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<FURNITURE>(GameEvents.FURNITURE_CHANGED, InitializeInventory);
    }
}

public enum FURNITURE
{
    SOFA,
    PLANT,
    WALL,
    FLOOR,
    PICTURE
}
