using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPlayer : MonoBehaviour
{
    public GameObject gameObjectToClamp; 

    public TextMeshProUGUI nickTxt;
    public GameObject addFriendsButton;

    public void Initialize(string text, bool isMyPlayer, GameObject clampTogameobject)
    {
        nickTxt.text = text;
        addFriendsButton.SetActive(!isMyPlayer);
        gameObjectToClamp = clampTogameobject;
    }

    private void Update()
    {
        if(gameObjectToClamp!= null)
        {
            transform.position = gameObjectToClamp.transform.position;
        }
    }

}
