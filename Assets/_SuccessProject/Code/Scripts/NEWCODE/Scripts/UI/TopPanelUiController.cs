using CMS.Config;
using SocialGTA;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelUiController : MonoBehaviour
{
    [SerializeField] Image filledLine;
    [SerializeField] TextMeshProUGUI lvlText;
    [SerializeField] TextMeshProUGUI softCurrency;
    [SerializeField] TextMeshProUGUI hardCurrency;
    [SerializeField] TextMeshProUGUI NickName;

    private SaveManager saveManager;

    private void Awake()
    {
        saveManager = SaveManager.Instance;

        Messenger.AddListener<int, int>(GameEvents.EXP_CHANGED, OnExpChanged); //playerLvl events
        Messenger.AddListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);
        Messenger.AddListener(GameEvents.CURRENCY_UPDATED, UpdateCurrency);

        Refresh();
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<int, int>(GameEvents.EXP_CHANGED, OnExpChanged);
        Messenger.RemoveListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);
        Messenger.RemoveListener(GameEvents.CURRENCY_UPDATED, UpdateCurrency);
    }

    private void Start()
    {
        AutorizationController autorization = new AutorizationController();
        autorization.Login();
        NickName.text = autorization.profile.UserName;
    }

    private void Refresh()
    {
        var exp = saveManager.GetExp();
        var expToNextLvl = saveManager.GetExpToNextLevel();
        filledLine.fillAmount = NormilizeExperienceForUI(exp, expToNextLvl);
        lvlText.text = saveManager.GetLvl().ToString();
        softCurrency.text = saveManager.GetSoftCurrency().ToString();
        hardCurrency.text = saveManager.GetHardCurrency().ToString();
    }

    private void UpdateCurrency()
    {
        softCurrency.text = saveManager.GetSoftCurrency().ToString();
        hardCurrency.text = saveManager.GetHardCurrency().ToString();
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
}
