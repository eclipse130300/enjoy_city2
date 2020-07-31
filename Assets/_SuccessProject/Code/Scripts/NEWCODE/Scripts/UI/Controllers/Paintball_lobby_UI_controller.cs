using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Paintball_lobby_UI_controller : MonoBehaviour
{
    [SerializeField] List<GameObject> pedestals = new List<GameObject>();
    [SerializeField] List<RectTransform> playerInfos = new List<RectTransform>();
    [SerializeField] RectTransform playerInfoRect;
    [SerializeField] GameObject playerInfoPlaceHolder;
    private const int teamsAmount = 2;
    [SerializeField] PaintBallTeam[] teams = new PaintBallTeam[teamsAmount];
    [SerializeField] Color[] teamColors = new Color[teamsAmount];
    [SerializeField] int maxPlayersInTeam = 4;

    void InitializeTeams()
    {
        var allTEams = Enum.GetValues(typeof(TEAM)).Cast<TEAM>().ToList();

        for (int i = 0; i < allTEams.Count; i++)
        {
            teams[i] = new PaintBallTeam(teamColors[i], allTEams[i], maxPlayersInTeam);
        }
    }

    private Dictionary<GameObject, RectTransform> rectToGameobject = new Dictionary<GameObject, RectTransform>();

    private void Start()
    {
        InitializeTeams();
    }

    public void OnNewPlayerConnected(int playersAmount)
    {
        //we check every team
        int theLessPlayerNum = maxPlayersInTeam;
        PaintBallTeam teamToJoin = null;

        foreach(PaintBallTeam team in teams)
        {
            if (team.playersInTeam.Count < theLessPlayerNum)
            {
                theLessPlayerNum = team.playersInTeam.Count;
                teamToJoin = team;
            }
        }

        SaveManager saveManager = SaveManager.Instance;

        PaintBallPlayer newPlayer = new PaintBallPlayer(saveManager.LoadBody(), saveManager.LoadClothesSet(saveManager.LoadBody().gender.ToString() + GameMode.Paintball.ToString()), saveManager.GetNickName());

        teamToJoin.JoinTeam(newPlayer);

        //if one team has less players than another


        //and players in team less than maxTeamPlayers - playersAmount


        //join team

    }

    public void OnPlayerDisconected()
    {

    }
}