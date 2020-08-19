using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour, IOnEventCallback
{
    public TEAM currentTeam;
    public int myTeamIndex;
    public Color teamColor;

    public string enemyTag = "Enemy";

    public PaintBallPlayer myPlayer;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void InitializePlayerTeam(int teamIndex, TEAM team, string colorHex)
    {
        myTeamIndex = teamIndex;
        currentTeam = team;
        Color playerCol;
        if (ColorUtility.TryParseHtmlString(colorHex, out playerCol)) teamColor = playerCol;

    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;

        if (eventCode == GameEvents.START_PAINTBALL_GAME)
        {
            //at the pre-beginning of our game let's set tags
            ApplyProperTagsToPlayers();
        }
    }

    private void ApplyProperTagsToPlayers()
    {
        var allPlayers = FindObjectsOfType<PlayerTeam>();

        foreach (PlayerTeam player in allPlayers)
        {
            if(player.myTeamIndex != this.myTeamIndex) //we check if players team isn't ours
            {
                player.gameObject.tag = enemyTag; //we apply enemy tag (for processing bullet)
            }
        }
    }
}



