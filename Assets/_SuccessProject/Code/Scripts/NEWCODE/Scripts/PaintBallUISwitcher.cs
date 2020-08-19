using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallUISwitcher : MonoBehaviour
{
    public GameObject lobbyUI;
    public GameObject gameUI;
        
    public void SwitchToLobbyUI()
    {
        gameUI.SetActive(false);
        lobbyUI.SetActive(true);
    }

    public void SwitchToGameUI()
    {
        lobbyUI.SetActive(false);
        gameUI.SetActive(true);
    }
}
