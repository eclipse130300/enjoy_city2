﻿using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//TODO refactor this!

public class TopPanelUiController : MonoBehaviour
{
    [SerializeField] Image filledLine;
    [SerializeField] TextMeshProUGUI lvlText;
    [SerializeField] TextMeshProUGUI softCurrency;
    [SerializeField] TextMeshProUGUI hardCurrency;

    private SaveManager saveManager;

    private void Awake()
    {
        saveManager = SaveManager.Instance;

        Messenger.AddListener<int, int>(GameEvents.EXP_CHANGED, OnExpChanged); //playerLvl events
        Messenger.AddListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);
        Messenger.AddListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, UpdateCurrency);
        Messenger.AddListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, UpdateCurr);
        Messenger.AddListener(GameEvents.UPDATE_SAND_BOX_UI, UpdateUI);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var exp = saveManager.GetExp();
        var expToNextLvl = saveManager.GetExpToNextLevel();
        filledLine.fillAmount = NormilizeExperienceForUI(exp, expToNextLvl);
        lvlText.text = saveManager.GetLvl().ToString();
        softCurrency.text = saveManager.GetSoftCurrency().ToString();
        hardCurrency.text = saveManager.GetHardCurrency().ToString();
    }

    private void UpdateCurrency(ItemConfig cfg, ItemVariant var) //TODO cfg && var is unnecessary
    {
        softCurrency.text = saveManager.GetSoftCurrency().ToString();
        hardCurrency.text = saveManager.GetHardCurrency().ToString();
    }

    private void UpdateCurr(RoomItemConfig rcgf, ItemVariant var) //это просто б копия функции сверху - ужасный костыль
    {
        softCurrency.text = saveManager.GetSoftCurrency().ToString();
        hardCurrency.text = saveManager.GetHardCurrency().ToString();
    }

    private void UpdateUI() // слить два метода сверху в этот
    {
        softCurrency.text = saveManager.GetSoftCurrency().ToString();
        hardCurrency.text = saveManager.GetHardCurrency().ToString();
        // добавить в него остальные элементы панели
    }

    private void OnLvlChanged(int lvl)
    {
        lvlText.text = lvl.ToString();
    }

    private void OnExpChanged(int exp, int expToNLvl)
    {
        filledLine.fillAmount = NormilizeExperienceForUI(exp, expToNLvl);
    }

    private float NormilizeExperienceForUI(int exp, int expToNextLvl)
    {
        return (float)exp / expToNextLvl;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int, int>(GameEvents.EXP_CHANGED, OnExpChanged);
        Messenger.RemoveListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);
        Messenger.RemoveListener<ItemConfig, ItemVariant>(GameEvents.ITEM_BOUGHT, UpdateCurrency);
        Messenger.AddListener<RoomItemConfig, ItemVariant>(GameEvents.ROOM_ITEM_BOUGHT, UpdateCurr);
    }
}
