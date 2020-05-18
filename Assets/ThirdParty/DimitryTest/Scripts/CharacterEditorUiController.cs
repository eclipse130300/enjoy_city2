using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditorUiController : MonoBehaviour
{
    [SerializeField] MapConfig playerRoom;
    [SerializeField] GameObject rightPanel;
    [SerializeField] Transform parentForItem;

    private GameObject itemDisplaying;

    [SerializeField] private Vector2 sizeToDisplay = new Vector2(81, 84);

    private void Awake()
    {
        rightPanel.SetActive(false);
        Messenger.AddListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.AddListener(GameEvents.ITEM_UNPRESSED, HideItem);
    }

    private void HideItem()
    {
        Destroy(itemDisplaying);
        rightPanel.SetActive(false);
    }

    private void DisplayItem(GameObject itemGO)
    {
        rightPanel.SetActive(true);
        itemDisplaying = Instantiate(itemGO, rightPanel.transform.position,  Quaternion.identity);
        itemDisplaying.transform.SetParent(parentForItem);
        itemDisplaying.GetComponent<RectTransform>().ResetTransform();
        itemDisplaying.GetComponent<RectTransform>().sizeDelta = sizeToDisplay;
        Destroy(itemDisplaying.GetComponent<ItemDisplay>());       
    }

    public void OnDoneButtonTap()
    {
        Loader.Instance.LoadGameScene(playerRoom);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.ITEM_PRESSED, DisplayItem);
        Messenger.RemoveListener(GameEvents.ITEM_UNPRESSED, HideItem);
    }


}
