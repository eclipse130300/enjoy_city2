using CMS.Config;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterInventoryManager : BaseInventoryManager
{
    List<ItemConfig> currentConfig;
    List<ItemConfig> inventory; //
    public ScriptableList<ItemConfig> SLinstance;

    public GameMode currentMode;
    public BODY_PART currentbodyPart;
    public Gender characterGender;


    protected override void Awake()
    {
        base.Awake();

        SLinstance = ScriptableList<ItemConfig>.instance;

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
        InitializeInventory(GameMode.SandBox, BODY_PART.HAIR);
    }


    private void InitializeInventory(GameMode mode, BODY_PART part)
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

    protected override void DisplayAppropriateItems() //here is a diff
    {
        foreach (ItemConfig cfg in  inventory) //insantiate in inv
        {
            var item = Instantiate(ItemPrefab);
            var itemScript = item.GetComponent<ItemDisplay>();
            item.transform.SetParent(contentObject);


            itemScript.itemConfig = cfg; //initialize
            itemScript.SetItem(cfg.Inventory_image, cfg.Inventory_frameColor, CheckIfItemIsActive(cfg));
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

    private bool CheckIfItemIsActive(ItemConfig cfg)
    {
        ClothesConfig activeClothes = SaveManager.Instance.LoadClothesSet(characterGender.ToString() + currentMode.ToString());
        if(activeClothes.ItemIsInConfig(cfg))
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
        //sort for 3d char items
        inventory = SLinstance.list.
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

