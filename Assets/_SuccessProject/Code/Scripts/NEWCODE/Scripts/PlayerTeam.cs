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
    private PhotonView photon;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void InitializePlayerTeam(PaintBallTeam team, Color teamCol)
    {
        myTeamIndex = team.teamIndex;
        currentTeam = team.teamName;
        teamColor = teamCol;
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
        if (!photon.IsMine) return;

        var allPlayers = FindObjectsOfType<PlayerTeam>();

        foreach (PlayerTeam player in allPlayers)
        {
            if(player.myTeamIndex != myTeamIndex) //we check if players team isn't ours
            {
                player.gameObject.tag = enemyTag; //we apply enemy tag (for processing bullet)
                player.gameObject.layer = 0;
            }
                //apply ally tag later?
        }
    }
}



