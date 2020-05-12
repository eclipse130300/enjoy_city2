using CMS.Config;
using SocialGTA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryPoint : MonoBehaviour, IIneractable
{
    private Loader loader;
    private SpriteRenderer miniMapPointRenderer;
    private bool isListening;

    [SerializeField] private int fromLevelAvailable;
    [SerializeField] private MapConfig mapCFG;
    [SerializeField] private Sprite interactionIcon;
    [SerializeField] private Sprite miniMapIcon;

    public void Interact()
    {
        loader.LoadGameScene(mapCFG);
    }

    private void Awake()
    {
        loader = Loader.Instance;

        miniMapPointRenderer = GetComponentInChildren<SpriteRenderer>();
        miniMapPointRenderer.sprite = miniMapIcon;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerLevel>()?.Level >= fromLevelAvailable)
        {
            Messenger.Broadcast(GameEvents.ENTRY_POINT_ENTERED, interactionIcon);
            Messenger.AddListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
            isListening = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerLevel>()?.Level >= fromLevelAvailable)
        {
            Messenger.Broadcast(GameEvents.ENTRY_POINT_EXIT);
            Messenger.RemoveListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
            isListening = false;
        }

    }

    private void OnDestroy()
    {
        if(isListening) Messenger.RemoveListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
    }

}
