using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallGameManager : MonoBehaviourSingleton<PaintBallGameManager>, IOnEventCallback, IHaveCooldown
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
    public int pointsToWin;

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
        GameFinishCheck();
    }

    void GameFinishCheck()
    {
        foreach (PaintBallTeam team in paintballTM.teams)
        {
            if (team.gamePoints >= pointsToWin)  //...teamManager knows about GM?
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
    }


    void StartGame()
    {
        Debug.Log("GAME STARTS!");
        InitializeTeamScore();
    }
}
