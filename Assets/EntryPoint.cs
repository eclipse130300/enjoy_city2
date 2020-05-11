using CMS.Config;
using SocialGTA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryPoint : MonoBehaviour , IIneractable
{
    private Loader loader;
    private SpriteRenderer miniMapPointRenderer;

    [SerializeField] private int fromLevelAvailable;
    [SerializeField] private MapConfig mapCFG;
    [SerializeField] private Sprite interactionIcon;
    [SerializeField] private Sprite miniMapIcon;

    private void Awake()
    {
        loader = Loader.Instance;

        miniMapPointRenderer = GetComponentInChildren<SpriteRenderer>();
        miniMapPointRenderer.sprite = miniMapIcon;

        Messenger.AddListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerLevel>()?.Level >= fromLevelAvailable)
        {
            Messenger<Sprite>.Broadcast(GameEvents.ENTRY_POINT_ENTERED, interactionIcon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerLevel>()?.Level >= fromLevelAvailable)
        {
            Messenger.Broadcast(GameEvents.ENTRY_POINT_EXIT);
        }
    }

    public void Interact()
    {
        loader.LoadGameScene(mapCFG);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvents.INTERACTION_BUTTON_TAP, Interact);
    }
}
