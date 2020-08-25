using CMS.Config;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameSpawner : MonoBehaviourSingleton<PaintBallGameSpawner>
{
    public PaintBallSpawnPoint[] spawnPoints;
    private PaintBallTeamManager paintBallTeamManager;
    public string mySceneName;

    [SerializeField] PhotonView photon;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();

        paintBallTeamManager = FindObjectOfType<PaintBallTeamManager>();
    }

    private void OnEnable()
    {
        Loader.Instance.AllSceneLoaded += InstantinateOnScenesLoaded;
    }

    private void OnDisable()
    {
        Loader.Instance.AllSceneLoaded -= InstantinateOnScenesLoaded;
    }

    private Vector3 PickStartSpawnPoint(PaintBallPlayer player)
    {
        //we take pedestal index as spawnpoint
        PaintBallSpawnPoint point = spawnPoints.Where(x => x.startSpawnIndex == player.myPedestalIndex).ToList()[0];
        //pick random one

        return point.gameObject.transform.position;
    }

    private void InstantinatePlayer(Vector3 point) //localfunc
    {
        //we stop sending ready messeges to master
        StopAllCoroutines();

        //instantinate player
        GameObject player = PhotonNetwork.Instantiate("PaintballPlayer", point, Quaternion.identity);

        PhotonNetwork.LocalPlayer.TagObject = player;

        //local stuff
        GameObject playerCam = player.GetComponentInChildren<PlayerCamera>().gameObject;
        Messenger.Broadcast(GameEvents.PAINTBALL_PLAYER_SPAWNED, playerCam);

        NotifyMasterIamReady();
    }

    void InstantinateOnScenesLoaded()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        //initially we pick spawnPoint based on pedestal index num
        InstantinatePlayer(PickStartSpawnPoint(paintBallTeamManager.myPlayer));
    }

    private void NotifyMasterIamReady()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(GameEvents.PLAYER_IS_READY_PAINTBALL_GAME, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private Vector3 PickRandomSpawnPoint(TEAM playerTeam)
    {
        //get all avalilible points
        List<PaintBallSpawnPoint> availiblePoints = spawnPoints.Where(x => x.team == playerTeam).Where(x => x.isOccupied == false).ToList();
        //pick random one
        int randomIndex = Random.Range(0, availiblePoints.Count);
        PaintBallSpawnPoint randomPoint = availiblePoints[randomIndex];
        //occupy this point
        randomPoint.isOccupied = true;

        return randomPoint.gameObject.transform.position;
    }

    public void RespawnPlayer(GameObject player, TEAM playerTeam)
    {
        var point = PickRandomSpawnPoint(playerTeam);
        player.transform.position = point;
    }

}
