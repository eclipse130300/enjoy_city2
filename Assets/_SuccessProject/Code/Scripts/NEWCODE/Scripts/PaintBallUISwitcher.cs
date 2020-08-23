using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallUISwitcher : MonoBehaviourSingleton<PaintBallUISwitcher>
{
    public GameObject lobbyUI;
    public GameObject gameUI;

    public GameObject lobbyResults;

/*    public void SwitchToLobbyUI()
    {
        gameUI.SetActive(false);
        lobbyUI.SetActive(true);
    }*/

    public void SwitchToGameUI()
    {
        lobbyUI.SetActive(false);
        gameUI.SetActive(true);
    }

    public void SwitchToFinishLobbyUI()
    {
        gameUI.SetActive(false);
        lobbyUI.SetActive(true);

        //show results
        lobbyResults.SetActive(true);
    }
}
