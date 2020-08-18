using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameManager : MonoBehaviour, IOnEventCallback
{
    public Dictionary<int, bool> readyList = new Dictionary<int, bool>();

    private bool PlayersAreReady
    {
        get { return readyList.Keys.Count == PhotonNetwork.CurrentRoom.PlayerCount; }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PLAYER_IS_READY_PAINTBALL_GAME)
        {
            //when player initially spawns(gameSpawner) it sends notification to master, who adds player to ready list
            if (!PlayersAreReady)
            {
                {
                    object[] data = (object[])photonEvent.CustomData;
                    bool value = (bool)data[0];
                    int senderKey = photonEvent.Sender;

                    Debug.Log("SENDER - " + senderKey + " VALUE - " + value);

                    AddToReadyList(senderKey, value);
                    Debug.Log("I added - " + senderKey + value);
                }
            }
        }
        else if(eventCode == GameEvents.START_GAME)
        {
            StartGame();
        }
    }

    private void AddToReadyList(int key, bool value)
    {
        if (!readyList.Keys.ToList().Contains(key))
        {
            readyList.Add(key, value);
            AllPlayersReadyCheck();
        }
    }

    void AllPlayersReadyCheck()
    {
        if (PlayersAreReady)
        {
            //timer ticks, and when ends, it sends us an event to start the game 
            UIstartTimer();
        }
    }

    void UIstartTimer()
    {
        /*        uiController.StartGameTimer();*/
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.START_CD_GAME_TIMER, content, raiseEventOptions, SendOptions.SendReliable);
    }

    void StartGame()
    {
        Debug.Log("GAME STARTS!");
    }
}
