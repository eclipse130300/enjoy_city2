using CMS.Config;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StartPaintball : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject startButton;
    [SerializeField] int minPlayers = 2;

    [SerializeField] float cDTime = 5.99f;
    [SerializeField] TextMeshProUGUI timerText;

    public Dictionary<int, bool> readyList = new Dictionary<int, bool>();

    public PhotonView photon;
    public MapConfig paintBallGame;

/*    private PaintBallUISwitcher UIswithcer;*/

    private void Awake()
    {
        photon = GetComponent<PhotonView>();
/*        PhotonNetwork.IsMessageQueueRunning = true; //if we return to lobby from game - turn it on here*/
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        startButton.SetActive(false);
    }

    public void ToggleStartButton(int currentPlayers)
    {
        bool value = EnoughPlayersToStart(currentPlayers, minPlayers);
        startButton.SetActive(value);
    }

    public override void OnJoinedRoom()
    {
        ToggleStartButton(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnLeftRoom()
    {
        ToggleStartButton(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ToggleStartButton(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ToggleStartButton(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private bool EnoughPlayersToStart(int currentPlayers, int minPlayerToStart)
    {
        bool value = currentPlayers >= minPlayerToStart ? true : false;
        return value;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PLAYER_IS_READY_PAINTBALL_LOBBY)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool value = (bool)data[0];
            int senderKey = photonEvent.Sender;

/*            Debug.Log("SENDER - " + senderKey + " VALUE - " + value);*/

            AddToReadyList(senderKey, value);
        }
    }

    private void AddToReadyList(int key, bool value)
    {
        foreach (int k in readyList.Keys.ToList())
        {
            if (k == key)
            {
                readyList.Remove(key);
                //abrupt CD here?
                photon.RPC("AbruptCountDown", RpcTarget.AllViaServer);
            }
        }
        readyList.Add(key, value);

        StartTimerCheck();
    }

    void StartTimerCheck()
    {
        if (AllPlayersReadyCheck())
        {
            //here we as master send RPC to start CD to allPlayers(including us)
            photon.RPC("BeginCountDown", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void BeginCountDown()
    {
        StartCoroutine(TimerRoutine());
    }

    [PunRPC]
    private void AbruptCountDown()
    {
            timerText.gameObject.SetActive(false);
            StopAllCoroutines();
    }

    [PunRPC]
    private void StartGame()
    {
/*
        //master save in roomProperties final teams
        if(PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties.Add("teams", JsonConvert.SerializeObject(PaintBallTeamManager.teams));
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }*/

        PhotonNetwork.IsMessageQueueRunning = false;
        Loader.Instance.LoadGameScene(paintBallGame);
/*        Destroy(this);*/


    }


    IEnumerator TimerRoutine()
    {
        //show timer to everyone
        float targetTime = (float)PhotonNetwork.Time + cDTime;
        timerText.gameObject.SetActive(true);

        while ((targetTime - PhotonNetwork.Time) > 0)
        {
            //tickTimer
            float timeLeft = Mathf.Max(targetTime - (float)PhotonNetwork.Time, 0f);
            timerText.text = Mathf.FloorToInt(timeLeft).ToString();

            yield return null;
        }

        //loadScene for everyone if everybody is in here
        if(PhotonNetwork.IsMasterClient)
        {
            photon.RPC("StartGame", RpcTarget.AllViaServer);
        }
        
    }

    private bool AllPlayersReadyCheck()
    {
        var readyListCount = readyList.Keys.Count;
/*        Debug.Log("readyListCount = " + readyListCount);*/
        if (readyListCount != PhotonNetwork.CurrentRoom.PlayerCount) return false; //all players didn't even press ready

        int readyCount = 0;
        foreach(bool value in readyList.Values)
        {
            if(value == true)
            {
                readyCount++;
            }
        }

/*        Debug.Log("READY COUNT = " + readyCount);*/
        if(readyCount == readyListCount)
        {
            return true;
        }

        return false;
    }
}
