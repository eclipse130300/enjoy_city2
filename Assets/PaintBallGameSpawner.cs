using CMS.Config;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameSpawner : MonoBehaviour
{
    public PaintBallSpawnPoint[] spawnPoints;

    public MapConfig paintBallGameConfig;

    public PaintBallTeam[] teams;

    [SerializeField] PhotonView photon;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();

        Loader.Instance.AllSceneLoaded += ResumeQueue;
    }

    private void OnDestroy()
    {
        Loader.Instance.AllSceneLoaded -= ResumeQueue;
    }

    void InitializeSpawnPoints()
    {
        spawnPoints = FindObjectsOfType<PaintBallSpawnPoint>();
    }

    void ResumeQueue()
    {
        if (Loader.Instance.curentScene != paintBallGameConfig) return;

        InitializeSpawnPoints();

        //resumeQueue, we loaded everything
        PhotonNetwork.IsMessageQueueRunning = true;

        if(PhotonNetwork.IsMasterClient)
        {
            //lets send spawnpoints to everybody
            foreach(PaintBallTeam team in PaintBallTeamManager.teams)
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
    }

    private Vector3 PickSpawnPoint(PaintBallPlayer player)
    {
        //get all avalilible points
        List<PaintBallSpawnPoint> availiblePoints = spawnPoints.Where(x => x.team == player.playerTeam).Where(x => x.isOccupied == false).ToList();
        //pick random one
        int randomIndex = Random.Range(0, availiblePoints.Count);
        PaintBallSpawnPoint randomPoint = availiblePoints[randomIndex];
        //occupy this point
        randomPoint.isOccupied = true;

        return randomPoint.gameObject.transform.position;
    }

    [PunRPC]
    private void InstantinatePlayer(Vector3 point)
    {
        //foreach player in each team it send RPC to targetPlayer with spawn point

        //instantinate player
        GameObject player = PhotonNetwork.Instantiate("Player", point, Quaternion.identity);

        //in skins manager change gameMode to paintball 
        player.GetComponent<SkinsManager>()._gameMode = GameMode.Paintball;
    }

}
