using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallTeamManager : MonoBehaviourPunCallbacks
{
    private const int teamsAmount = 2;

    public PaintBallTeam[] teams = new PaintBallTeam[teamsAmount];
    [SerializeField] List<GameObject> pedestals = new List<GameObject>();

    [SerializeField] Color[] teamColors = new Color[teamsAmount];
    [SerializeField] int maxPlayersInTeam = 4;

    public PaintBallPlayer myPlayer;
    public PaintBallTeam myTeam;

    public override void OnEnable()
    {
        Loader.Instance.AllSceneLoaded += InitializeEverything;
    }

    public override void OnDisable()
    {
        Loader.Instance.AllSceneLoaded -= InitializeEverything;
    }

    private void InitializeEverything()
    {
        InitializePedestals();
        InitializeTeams();
    }

    public void SetMyPlayerAndMyTeam(PaintBallPlayer player, PaintBallTeam team)
    {
        myPlayer = player;
        myTeam = team;
    }

/*    public void InitializeTeamsViaRoomProps()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.IsNullOrEmpty()) return;

        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        var teamsSTR = (string)roomProps["teams"];
        teams = (PaintBallTeam[])JsonConvert.DeserializeObject(teamsSTR);
    }*/


    void InitializeTeams()
    {
            var allTEams = Enum.GetValues(typeof(TEAM)).Cast<TEAM>().ToList();
            /*        int arrayIndex = 0;*/

            for (int i = 0; i < allTEams.Count; i++)
            {
                GameObject[] teamPedestals = new GameObject[maxPlayersInTeam];
                pedestals.CopyTo(maxPlayersInTeam * i, teamPedestals, 0, maxPlayersInTeam);
                teams[i] = new PaintBallTeam(teamColors[i], allTEams[i], maxPlayersInTeam, teamPedestals, i);
            }
    }

    void InitializePedestals()
    {
        //if we find pedestals on the scene sort them by priority index
        if(pedestals.Count == 0)
        {
            var allPedestals = FindObjectsOfType<PedestalController>().ToList();
            int maxIterations = allPedestals.Count;

            for (int i = 0; i < maxIterations; i++)
            {
                PedestalController itemToadd = null;
                int minIndex = int.MaxValue;

                foreach (PedestalController item in allPedestals)
                {
                    if (item.pedestalID <= minIndex)
                    {
                        itemToadd = item;
                        minIndex = item.pedestalID;
                    }
                }
                pedestals.Add(itemToadd.gameObject);
                allPedestals.Remove(itemToadd);
            }
        }
    }

    public bool LookForEmptySlot()
    {
        int allPlayersConnected = 0;

        foreach (var team in teams)
        {
            allPlayersConnected += team.playersInTeam.Count;
        }

        return allPlayersConnected < teamsAmount * maxPlayersInTeam;
    }

    public PaintBallTeam PickTeam(/*PaintBallPlayer player*/)
    {
        //we check every team
        int theLessPlayerNum = maxPlayersInTeam;
        PaintBallTeam teamToJoin = null;

        //if one team has less players than another
        foreach (PaintBallTeam team in teams)
        {
            if (team.playersInTeam.Count < theLessPlayerNum)
            {
                theLessPlayerNum = team.playersInTeam.Count;
                teamToJoin = team;
            }
        }

        return teamToJoin;
    }

    public bool PlayerIsInTeam(int actorNum)
    {
        foreach (var team in teams)
        {
            foreach (PaintBallPlayer pl in team.playersInTeam)
            {
                if (pl.photonActorNumber == actorNum)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public PaintBallTeam AddPlayerToTeam(int teamIndex, PaintBallPlayer player)
    {
        PaintBallTeam playerNewTeam = null;

        foreach (var team in teams)
        {
            if (team.teamIndex == teamIndex)
            {
                playerNewTeam = team;
                playerNewTeam.JoinTeam(player);
            }
        }

        if (playerNewTeam == null)
        {
            Debug.LogError("CannotFind team by index");
            return null;
        }
        else
        {
            return playerNewTeam;
        }
    }

    public void RemovePlayerFromGame(int actorNum)
    {
        foreach(var pedestal in pedestals)
        {
            var controller = pedestal.GetComponent<PedestalController>();
            if(controller.currentPlayer.photonActorNumber == actorNum)
            {
                foreach(PaintBallTeam team in teams)
                {
                    foreach(PaintBallPlayer player in team.playersInTeam.ToList())
                    {
                        if(player.photonActorNumber == actorNum)
                        {
                            team.RemoveFromTeam(player);
                        }
                    }
                }

                controller.DeletePlayerAndInfo();
            }
        }
    }

    public PaintBallTeam GetTeamByIndex(int teamIndex)
    {
        foreach(PaintBallTeam team in teams)
        {
            if(team.teamIndex == teamIndex)
            {
                return team;
            }
        }
        return null;
    }
}

public enum TEAM
{
    RED,
    BLUE
}
