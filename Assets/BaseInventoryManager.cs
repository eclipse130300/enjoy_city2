using UnityEngine;

public abstract class BaseInventoryManager : MonoBehaviour
{

    public ShopManager shopManager;

    public int columnsCount;
    public int inventoryMinSize;
    public GameObject ItemPrefab;
    public GameObject EmptySlot;
    public Transform contentObject;

    protected virtual void Awake()
    {
        shopManager = ShopManager.Instance;
    }

    protected abstract void GetItems();
    protected abstract void DisplayAppropriateItems();

    protected void ClearInventory()
    {
        var children = contentObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != this.transform) Destroy(child.gameObject);
        }
    }

    protected void RefreshInventory()
    {
        ClearInventory();
        GetItems();
        DisplayAppropriateItems();
    }

    protected void InstantiateEmptyItem()
    {
        var emptyItem = Instantiate(EmptySlot);
        emptyItem.transform.SetParent(contentObject);
    }
}
