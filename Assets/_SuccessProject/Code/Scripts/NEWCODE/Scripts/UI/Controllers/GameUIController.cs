using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//responsible for interaction button
public class GameUIController : MonoBehaviour
{
    public GameObject interactionButton;
    public Image interactionButtonIMG;


    private void Awake()
    {
        Messenger.AddListener<Sprite>(GameEvents.ENTRY_POINT_ENTERED, OnEntryPointEntered); //entrypoint events
        Messenger.AddListener(GameEvents.ENTRY_POINT_EXIT, OnEntryPointExit);

        interactionButton.SetActive(false);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Sprite>(GameEvents.ENTRY_POINT_ENTERED, OnEntryPointEntered);
        Messenger.RemoveListener(GameEvents.ENTRY_POINT_EXIT, OnEntryPointExit);

        interactionButton.SetActive(false);
    }
    private void OnEntryPointExit()
    {
        interactionButton.SetActive(false);
    }

    private void OnEntryPointEntered(Sprite newSprite)
    {
        Debug.Log("OnEntryPointEntered");
        interactionButton.SetActive(true);
        interactionButtonIMG.sprite = newSprite;
    }

    public void OnInteractionButtonTap()
    {
        Messenger.Broadcast(GameEvents.INTERACTION_BUTTON_TAP);
    }

}
