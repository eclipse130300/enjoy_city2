using CMS.Config;
using SocialGTA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryPoint : MonoBehaviour
{
    private SpriteRenderer miniMapPointRenderer;

    public int fromLevelAvailable;
    private bool isListeningUI;
    private bool isActivePoint;

    [SerializeField] private MapConfig mapCFG;
    [SerializeField] private Sprite interactionIcon;
    [SerializeField] private Sprite miniMapIcon;


    public void Interact()
    {
        Loader.Instance.LoadGameScene(mapCFG);
    }

    private void Awake()
    {
        miniMapPointRenderer = GetComponentInChildren<SpriteRenderer>();
        miniMapPointRenderer.sprite = miniMapIcon;
    }


    public void ShowUI()
    {
            Messenger.Broadcast(GameEvents.ENTRY_POINT_ENTERED, interactionIcon);
    }

    public void HideUI()
    {
            Messenger.Broadcast(GameEvents.ENTRY_POINT_EXIT);
    }

    public void ListenInteractionButton()
    {
        Messenger.AddListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
        isListeningUI = true;
    }

    public void UnlistenInteractionButton()
    {

            Messenger.RemoveListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
            isListeningUI = false;
    }

    private void OnDestroy()
    {
        if(isListeningUI) Messenger.RemoveListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
        Messenger.Broadcast(GameEvents.ENTRY_POINT_EXIT);
    }
}
