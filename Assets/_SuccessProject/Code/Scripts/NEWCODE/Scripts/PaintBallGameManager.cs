using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameManager : MonoBehaviour, IOnEventCallback, IHaveCooldown
{
    public List<int> readyList = new List<int>();

    private bool PlayersAreReady
    {
        get { return readyList.Count == PhotonNetwork.CurrentRoom.PlayerCount; }
    }

    [Header("CdSystem")]
    [SerializeField] int iD;

    public int CoolDownId => iD;

    public float CoolDownDuration
    {
        get { return minutes * 60 + secounds; }
    }

    [Header("Game time")]
    [SerializeField] int minutes;
    [SerializeField] int secounds;

    [Header("PointsToWin")]
    [SerializeField] int pointsToWin;

    private PaintBallTeamManager paintBallTeamManager;
    //field gamescore

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        paintBallTeamManager = FindObjectOfType<PaintBallTeamManager>();
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
    }

    private void AddScoreToTeam(PaintBallTeam team, int pointsAmount)
    {
        team.gamePoints += pointsAmount;
        GameFinishCheck();
    }

    void InitializeTeamScore()
    {
        foreach (PaintBallTeam team in paintBallTeamManager.teams)
        {
            team.gamePoints = 0;
        }
    }

    void GameFinishCheck()
    {
        foreach(PaintBallTeam team in paintBallTeamManager.teams)
        {
            if(team.gamePoints >= pointsToWin)
            {
                GameFinishEvent(team);
            }
        }
    }

    void GameFinishEvent(PaintBallTeam team)
    {

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
        Debug.Log("start game CD event send!");
    }


    void StartGame()
    {
        Debug.Log("GAME STARTS!");
        //initialize teams points to 0
        InitializeTeamScore();

    }
}
