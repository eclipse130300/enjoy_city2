﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class FinishPaintBall : MonoBehaviour, IOnEventCallback
{

    [SerializeField] GameObject resultsPanel;
    [SerializeField] int softCurrencyBonusPercent = 30;


    public int myTeamID;

    private PlayerLevel playerLevel;

    private void Awake()
    {
        Loader.Instance.AllSceneLoaded += AddBonusExpOnGameFinish;

        myTeamID = PaintBallTeamManager.Instance.myTeam.teamIndex;
        playerLevel = GetComponent<PlayerLevel>();

        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        Loader.Instance.AllSceneLoaded -= AddBonusExpOnGameFinish;

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void AddBonusExpOnGameFinish()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.IsNullOrEmpty() && PhotonNetwork.LocalPlayer.CustomProperties.IsNullOrEmpty())
            {
                Debug.LogError("We have nothing to add or game isn't player yet.. return");
                return;
            }
            else
            {
                //we add final bonus exp here
                //add team exp
                var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
                int finalEXP = 0;

                if (roomProps.ContainsKey("Team" + myTeamID.ToString()))
                {
                    int teamEXP = (int)roomProps["Team" + myTeamID.ToString()];
                    finalEXP += teamEXP;
                    Debug.Log("FINAL_TEAM_EXP :" + teamEXP);
                }
                //add player exp
                var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;

                if (playerProps.ContainsKey("playerEXP"))
                {
                    int playerEXP = (int)playerProps["playerEXP"];
                    finalEXP += playerEXP;
                    Debug.Log("FINAL_PLAYER_EXP :" + playerEXP);
                }
                //add exp
                playerLevel.AddExperience(finalEXP);
                Debug.Log("I ADDED EXP FOR PAINTBALL. AMOUNT - " + finalEXP);

                //add soft currency
                int finalSoft = PercentageUtils.GetPercentageInt(softCurrencyBonusPercent, finalEXP);
                SaveManager.Instance.AddSoftCurrency(finalSoft);
                Debug.Log("I ADDED SOFT FOR PAINTBALL. AMOUNT - " + finalSoft);

                //show everything above to our player
                resultsPanel.SetActive(true);
                resultsPanel.GetComponent<PaintBallPointsPanel>().SetResult(finalEXP, finalSoft);
                //
            }



        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PAINTBALL_FINISHED)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                //master opens room again for other users
                PhotonNetwork.CurrentRoom.IsOpen = true;
                ClearRoomProps();
            }

            ClearPlayerProps();
        }
    }

    void ClearRoomProps()
    {
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProps.Clear();

        Debug.Log("RoomProps removed!");
    }

    void ClearPlayerProps()
    {
        //we just need to save the player info for future game, and delete everything else
        var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;

        var playerInfo = playerProps["playerWithTeam"];

        playerProps.Clear();
        playerProps.Add("playerWithTeam", playerInfo);

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("PlayerProps removed!");
    }
}
