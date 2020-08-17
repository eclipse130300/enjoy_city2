using CMS.Config;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameSpawner : MonoBehaviour  /*, IOnEventCallback*/
{
    public PaintBallSpawnPoint[] spawnPoints;
    private PaintBallTeamManager paintBallTeamManager;
    public string mySceneName;

    public Dictionary<int, bool> readyList = new Dictionary<int, bool>();

    private bool PlayersAreReady
    {
        get { return readyList.Keys.Count == PhotonNetwork.CurrentRoom.PlayerCount; }
    }

    [SerializeField] PhotonView photon;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();

        paintBallTeamManager = FindObjectOfType<PaintBallTeamManager>();
/*        if (spawnPoints.IsNullOrEmpty()) InitializeSpawnPoints();*/
    }

    private void OnEnable()
    {
        Loader.Instance.AllSceneLoaded += InstantinateOnScenesLoaded;
    }

    private void OnDisable()
    {
        Loader.Instance.AllSceneLoaded -= InstantinateOnScenesLoaded;
    }

    private Vector3 PickSpawnPoint(PaintBallPlayer player)
    {
        //we take pedestal index as spawnpoint
        PaintBallSpawnPoint point = spawnPoints.Where(x => x.index == player.myPedestalIndex).ToList()[0];
        //pick random one

        return point.gameObject.transform.position;
    }

    [PunRPC]
    private void InstantinatePlayer(Vector3 point)
    {
        //we stop sending ready messeges to master
        StopAllCoroutines();

        //instantinate player
        GameObject player = PhotonNetwork.Instantiate("PaintballPlayer", point, Quaternion.identity);

        //in skins manager change gameMode to paintball 
        player.GetComponent<SkinsManager>()._gameMode = GameMode.Paintball;
        GameObject playerCam = player.GetComponentInChildren<PlayerCamera>().gameObject;

        Messenger.Broadcast(GameEvents.PAINTBALL_PLAYER_SPAWNED, playerCam);
    }

    void InstantinateOnScenesLoaded()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        InstantinatePlayer(PickSpawnPoint(paintBallTeamManager.myPlayer));
    }


    /*    void InitializeSpawnPoints()
        {
            spawnPoints = FindObjectsOfType<PaintBallSpawnPoint>();
        }*/

/*    void StartReadyRoutine()
    {
        StartCoroutine(NotifyMasterIamReadyRoutine());
    }


    IEnumerator NotifyMasterIamReadyRoutine()
    {
        while (true) //let's notify master every one secound that we are ready(
        {
            Debug.Log("I SEND EVENT!");
            bool isReady = true;
            object[] content = new object[] { isReady };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
            PhotonNetwork.RaiseEvent(GameEvents.PLAYER_IS_READY_PAINTBALL_GAME, content, raiseEventOptions, SendOptions.SendReliable);

            yield return new WaitForSeconds(1f);
        }
    }

    void FindSpawnPointForEveryPlayer()
    {
            //lets send spawnpoints to everybody
            foreach(PaintBallTeam team in paintBallTeamManager.teams)
            {
                foreach(PaintBallPlayer player in team.playersInTeam)
                {
                    //we find point
                    Vector3 point = PickSpawnPoint(player);
                    // and we send this point via RPC
                    Player playerToSend = PhotonNetwork.CurrentRoom.GetPlayer(player.photonActorNumber);
                    photon.RPC("InstantinatePlayer", playerToSend, point);
                }
            }
    }

    private Vector3 PickSpawnPoint(PaintBallPlayer player)
    {
        //get all avalilible points
        List<PaintBallSpawnPoint> availiblePoints = spawnPoints.Where(x => x.team == player.teamName).Where(x => x.isOccupied == false).ToList();
        //pick random one
        int randomIndex = Random.Range(0, availiblePoints.Count);
        PaintBallSpawnPoint randomPoint = availiblePoints[randomIndex];
        //occupy this point
        randomPoint.isOccupied = true;

        return randomPoint.gameObject.transform.position;
    }


    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("I RECIEVE EVENT!");
        //when master loads it recieves notifications creates and adds player to ready list
        if (!PlayersAreReady)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == GameEvents.PLAYER_IS_READY_PAINTBALL_GAME)
            {
                object[] data = (object[])photonEvent.CustomData;
                bool value = (bool)data[0];
                int senderKey = photonEvent.Sender;

                *//*            Debug.Log("SENDER - " + senderKey + " VALUE - " + value);*//*

                AddToReadyList(senderKey, value);
                Debug.Log("I added - " + senderKey + value);
            }
        }
    }

    private void AddToReadyList(int key, bool value)
    {
        if(!readyList.Keys.ToList().Contains(key))
        {
            readyList.Add(key, value);
            AllPlayersReadyCheck();
        }       
    }

    void AllPlayersReadyCheck()
    {
        if(PlayersAreReady)
        {
            FindSpawnPointForEveryPlayer();
        }
    }*/
}
