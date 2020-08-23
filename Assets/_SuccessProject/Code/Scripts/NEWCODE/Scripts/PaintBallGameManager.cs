using CMS.Config;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameManager : MonoBehaviourSingleton<PaintBallGameManager>, IOnEventCallback
{
    public List<int> readyList = new List<int>();

    private bool gameIsActive;

    private bool PlayersAreReady
    {
        get { return readyList.Count == PhotonNetwork.CurrentRoom.PlayerCount; }
    }

    [Header("PointsToWin")]
    public int pointsToWin;

    [SerializeField] int gameEndsDelay = 5; //in secounds
    [SerializeField] MapConfig gameFinishedScene;
    PaintBallTeamManager paintballTM;

    private void Awake()
    {
        if (PaintBallTeamManager.Instance != null)
        {
            paintballTM = PaintBallTeamManager.Instance;
        }
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
                int senderKey = photonEvent.Sender;
                AddLoadedPlayerToReadyList(senderKey);

            }
        }
        else if(eventCode == GameEvents.START_PAINTBALL_GAME)
        {
            StartGame();
        }
        else if(eventCode == GameEvents.HIT_RECIEVED)
        {
                object[] data = (object[])photonEvent.CustomData;
                int actorNum = (int)data[0];
                int currentHP = (int)data[1];
                int dmgAmount = (int)data[2];
                int fromTeamId = (int)data[3];

                AddScoreToTeam(fromTeamId, dmgAmount);
        }
        else if(eventCode == GameEvents.PAINTBALL_GAME_FINISHED)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                //here we write results to room / player props
                //todo make awards count?
                var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            }
            StartCoroutine(AfterGameDelay(gameEndsDelay));
        }
    }

    private IEnumerator AfterGameDelay(int delay)
    {
        yield return new WaitForSeconds(delay);

        PhotonNetwork.IsMessageQueueRunning = false;
        PaintBallUISwitcher.Instance.SwitchToFinishLobbyUI();
        Loader.Instance.LoadGameScene(gameFinishedScene);
    }

    void InitializeTeamScore()
    {
        foreach (PaintBallTeam team in paintballTM.teams)
        {
            team.gamePoints = 0;
        }
    }

    private void AddScoreToTeam(int teamId, int pointsAmount)
    {
        var team = paintballTM.GetTeamByIndex(teamId);
        team.gamePoints += pointsAmount;
        Debug.Log("I Add " + pointsAmount + " to team " + team.teamName.ToString() + "now points are :" + team.gamePoints);

        GameFinishCheck();
    }

    public PaintBallTeam WhichTeamHasWon()
    {
        var allTeams = PaintBallTeamManager.Instance.teams;
        int maxPTS = 0;
        PaintBallTeam winnerTeam = null;

        foreach (PaintBallTeam team in allTeams)
        {
            if(team.gamePoints >= maxPTS)
            {
                maxPTS = team.gamePoints;
                winnerTeam = team;
            }
        }
        return winnerTeam;
    }

/*    void UpdateScoreEvent(int newScore)
    {
        object[] content = new object[] { newScore };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.PAINTBALL_GAME_FINISHED, content, raiseEventOptions, SendOptions.SendReliable);
    }*/

    void GameFinishCheck()
    {
        foreach (PaintBallTeam team in paintballTM.teams)
        {
            if (team.gamePoints >= pointsToWin && gameIsActive) 
            {
                gameIsActive = false;
                GameFinishEvent(team);
            }
        }
    }

    void GameFinishEvent(PaintBallTeam team)
    {
        Debug.Log("Game finished! winning team is: " + team.teamName.ToString() + " !");

        object[] content = new object[] { team.teamIndex };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.PAINTBALL_GAME_FINISHED, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void AddLoadedPlayerToReadyList(int key)
    {
        if (!readyList.Contains(key))
        {
            readyList.Add(key);
            AllPlayersReadyCheck();
        }
    }

    void AllPlayersReadyCheck()
    {
        if (PlayersAreReady)
        {
            //timer ticks, and when ends, it sends us an event to start the game 
            UIstartTimerEvent();
        }
    }

    void UIstartTimerEvent()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.START_CD_GAME_TIMER, content, raiseEventOptions, SendOptions.SendReliable);
    }

    void StartGame()
    {
        Debug.Log("GAME STARTS!");
        InitializeTeamScore();
        gameIsActive = true;


    }
}
