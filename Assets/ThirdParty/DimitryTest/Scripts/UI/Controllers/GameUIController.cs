using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    /*private Loader loader;*/
    public GameObject interactionButton;
    public Image interactionButtonIMG;
    public Image filledLine;
    public TextMeshProUGUI lvlText;

    private Image defaultInteractionButIMG;

    private void Awake()
    {
        Messenger<Sprite>.AddListener(GameEvents.ENTRY_POINT_ENTERED, OnEntryPointEntered); //entrypoint events
        Messenger.AddListener(GameEvents.ENTRY_POINT_EXIT, OnEntryPointExit);

        Messenger<float>.AddListener(GameEvents.EXP_CHANGED, OnExpChanged); //playerLvl events
        Messenger<int>.AddListener(GameEvents.LVL_CHANGED, OnLvlChanged);

    }

    private void Start()
    {
        interactionButton.SetActive(false);
        defaultInteractionButIMG = interactionButtonIMG;
    }

    private void OnDestroy()
    {
        Messenger<Sprite>.RemoveListener(GameEvents.ENTRY_POINT_ENTERED, OnEntryPointEntered);
        Messenger.RemoveListener(GameEvents.ENTRY_POINT_EXIT, OnEntryPointExit);

        Messenger<float>.RemoveListener(GameEvents.EXP_CHANGED, OnExpChanged);
        Messenger<int>.RemoveListener(GameEvents.LVL_CHANGED, OnLvlChanged);
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
        interactionButtonIMG = defaultInteractionButIMG;
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
