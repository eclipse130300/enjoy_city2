using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public GameObject interactionButton;
    public Image interactionButtonIMG;
    public Image filledLine;
    public TextMeshProUGUI lvlText;


    private void Awake()
    {
        Messenger.AddListener<Sprite>(GameEvents.ENTRY_POINT_ENTERED, OnEntryPointEntered); //entrypoint events
        Messenger.AddListener(GameEvents.ENTRY_POINT_EXIT, OnEntryPointExit);

        Messenger.AddListener<float>(GameEvents.EXP_CHANGED, OnExpChanged); //playerLvl events
        Messenger.AddListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);

        interactionButton.SetActive(false);
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Sprite>(GameEvents.ENTRY_POINT_ENTERED, OnEntryPointEntered);
        Messenger.RemoveListener(GameEvents.ENTRY_POINT_EXIT, OnEntryPointExit);

        Messenger.RemoveListener<float>(GameEvents.EXP_CHANGED, OnExpChanged);
        Messenger.RemoveListener<int>(GameEvents.LVL_CHANGED, OnLvlChanged);

        interactionButton.SetActive(false);
    }

    private void OnLvlChanged(int lvl)
    {
        lvlText.text = lvl.ToString();
    }

    private void OnExpChanged(float lineFill)
    {
        filledLine.fillAmount = lineFill;
    }

    private void OnEntryPointExit()
    {
        interactionButton.SetActive(false);
    }

    private void OnEntryPointEntered(Sprite newSprite)
    {
        interactionButton.SetActive(true);
        interactionButtonIMG.sprite = newSprite;
    }

    public void OnInteractionButtonTap()
    {
        Messenger.Broadcast(GameEvents.INTERACTION_BUTTON_TAP);
    }

}
