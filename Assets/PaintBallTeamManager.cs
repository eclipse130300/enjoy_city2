using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallTeamManager : MonoBehaviour
{
    private const int teamsAmount = 2;

    [SerializeField] PaintBallTeam[] teams = new PaintBallTeam[teamsAmount];
    [SerializeField] List<GameObject> pedestals = new List<GameObject>();
    [SerializeField] Color[] teamColors = new Color[teamsAmount];
    [SerializeField] int maxPlayersInTeam = 4;

    private void Start()
    {
        InitializeTeams();
    }

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

    public bool PlayerIsInTeam(string userID)
    {
        foreach (var team in teams)
        {
            foreach (PaintBallPlayer pl in team.playersInTeam)
            {
                if (pl.photonUserID == userID)
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

    public void RemovePlayerFromGame(string UserId)
    {
        foreach(var pedestal in pedestals)
        {
            var controller = pedestal.GetComponent<PedestalController>();
            if(controller.currentPlayer.photonUserID == UserId)
            {
                foreach(PaintBallTeam team in teams)
                {
                    foreach(PaintBallPlayer player in team.playersInTeam.ToList())
                    {
                        if(player.photonUserID == UserId)
                        {
                            team.RemoveFromTeam(player);
                        }
                    }
                }

                controller.DeletePlayerAndInfo();
            }
        }
    }
}
