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
/*        int arrayIndex = 0;*/

        for (int i = 0; i < allTEams.Count; i++)
        {
            GameObject[] teamPedestals = new GameObject[maxPlayersInTeam];
            pedestals.CopyTo(maxPlayersInTeam * i, teamPedestals, 0, maxPlayersInTeam);


            teams[i] = new PaintBallTeam(teamColors[i], allTEams[i], maxPlayersInTeam, teamPedestals);

/*            arrayIndex += maxPlayersInTeam; */
        }
    }

    private Dictionary<GameObject, RectTransform> rectToGameobject = new Dictionary<GameObject, RectTransform>();

    private void Start()
    {
        InitializeTeams();
    }

    public void OnNewPlayerConnected(int playersAmount = 1)
    {
        //we check every team
        int theLessPlayerNum = maxPlayersInTeam;
        PaintBallTeam teamToJoin = null;

        //if one team has less players than another

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

        //join team
        teamToJoin.JoinTeam(newPlayer);

        //we pick first empty pedestal
        var pedestal = newPlayer.GetTeamPedestal(teamToJoin);

        //we spawn body and put nick above his head
        pedestal.GetComponent<PedestalController>().SpawnPlayerAndInfo(newPlayer);
    }

    public void OnPlayerDisconected()
    {

    }
}