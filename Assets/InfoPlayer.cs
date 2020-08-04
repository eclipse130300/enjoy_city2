using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPlayer : MonoBehaviour
{
    public TextMeshProUGUI nickTxt;
    public GameObject addFriendsButton;

    public void Initialize(string text, bool photonIsMine)
    {
        nickTxt.text = text;
        addFriendsButton.SetActive(!photonIsMine);
    }

}
