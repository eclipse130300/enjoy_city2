using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelUiController : MonoBehaviour
{
    public Image filledLine;
    public TextMeshProUGUI lvlText;
    private SaveManager saveManager;

    private void Awake()
    {
        Messenger.AddListener<float>(GameEvents.EXP_CHANGED, OnExpChanged); //playerLvl events
        Messenger.AddListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);
    }

    private void Start()
    {
/*
        lvlText.text = SaveManager.Instance.GetLvl().ToString();*/
    }


    private void OnDestroy()
    {
        Messenger.RemoveListener<float>(GameEvents.EXP_CHANGED, OnExpChanged);
        Messenger.RemoveListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);
    }

    private void OnLvlChanged(int lvl)
    {
        lvlText.text = lvl.ToString();
    }

    private void OnExpChanged(float lineFill)
    {
        filledLine.fillAmount = lineFill;
    }
}
