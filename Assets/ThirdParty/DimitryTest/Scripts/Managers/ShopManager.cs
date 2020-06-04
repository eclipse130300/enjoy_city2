using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    private SaveManager saveManager;

    private void Awake()
    {
        saveManager = SaveManager.Instance;
    }

    public bool CheckIsEnoughMoney(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.SOFT:
                if (saveManager.GetSoftCurrency() >= amount)
                {
                    return true;
                }
                break;
            case CurrencyType.HARD:
                if (saveManager.GetHardCurrency() >= amount)
                {
                    return true;
                }
                break;
        }
        return false;
    }
    //save manager buys?
    public void Buy(ItemConfig cfg, ItemVariant varitant, int cost, CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.SOFT:
                var Scurrency = saveManager.GetSoftCurrency();
                Scurrency -= cost;
                saveManager.SetSoftCurrency(Scurrency);
                break;
            case CurrencyType.HARD:
                var Hcurrency = saveManager.GetHardCurrency();
                Hcurrency -= cost;
                saveManager.SetHardCurrency(Hcurrency);
                break;
        }
        saveManager.Add3DItemToShopList(cfg, varitant);
    }

    public bool CheckIfItemIsBought(ItemConfig cfg, ItemVariant variant) //check if bought item+concrete var
    {
        var list = saveManager.Get3DItemList();

        foreach(string str in list)
        {
            var pair = str.Split('+');
            if(pair[0].Contains(cfg.ConfigId) && pair[1].Contains(variant.ConfigId))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfItemIsBought(RoomItemConfig cfg) // overload check if bought item only
    {
        var list = saveManager.GetRoomItemList();

        foreach (string str in list)
        {
            var pair = str.Split('+');
            if (pair[0].Contains(cfg.ConfigId))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfItemIsBought(ItemConfig cfg) // overload check if bought item only
    {
        var list = saveManager.Get3DItemList();

        foreach (string str in list)
        {
            var pair = str.Split('+');
            if (pair[0].Contains(cfg.ConfigId))
            {
                return true;
            }
        }
        return false;
    }
}
