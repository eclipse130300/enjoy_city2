using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallBonusEXPManager : MonoBehaviour, IOnEventCallback
{
    [SerializeField] float killStreakTime = 10f;

    [Header("PlayersKillstreaksPTS")]
    [SerializeField]const int EXPFORASSIST = 5;
    [SerializeField]const int EXPFORSINGLEKILL = 15;
    [SerializeField]const int EXPFORDOUBLEKILL = 20;
    [SerializeField]const int EXPFORTRIPPLEKILL = 30;
    [SerializeField]const int EXPFORULTRAKILL = 40;
    [SerializeField]const int EXPFORRAMPAGE = 50;

    private int killStreakCount = 0;

    private Coroutine killStreakRoutine = null;

    [Header("TeamOverallBonusPTS")] //here is hardCode...todo PAINTBALL SCRIPTABLE CONFIG???
    [SerializeField]const int EXP_FOR_100_TEAM_DMG = 15;
    [SerializeField]const int EXP_FOR_500_TEAM_DMG = 50;
    [SerializeField]const int EXP_FOR_1000_TEAM_DMG = 75;
    [SerializeField]const int EXP_FOR_2000_TEAM_DMG = 150;
    [SerializeField]const int EXP_FOR_3000_TEAM_DMG = 250;

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
        if (eventCode == GameEvents.PLAYER_DEATH)
        {
            object[] data = (object[])photonEvent.CustomData;
            int killerNum = (int)data[0];
            int[] damagers = (int[])data[1];

            Debug.Log("SOME PLAYER SEEMS TO BE DEAD... killer :" + killerNum + " damagersLength " + damagers.Length);

            CheckForAssist(damagers);
            CheckForKill(killerNum);
        }
        else if (eventCode == GameEvents.HIT_RECIEVED)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                object[] data = (object[])photonEvent.CustomData;
/*                int actorNum = (int)data[0];
                int currentHP = (int)data[1];
                int dmgAmount = (int)data[2];*/
                int fromTeamId = (int)data[3];

                AddBonusTeamEXP(fromTeamId);
            }
        }
    }

    private void CheckForAssist(int [] damagers)
    {
        foreach (int damagerActorNum in damagers)
        {
            if(PhotonNetwork.LocalPlayer.ActorNumber == damagerActorNum)
            {
                
                AddToPlayerProps(EXPFORASSIST);
            }
        }
    }

    private void CheckForKill(int killerNum)
     {
        if (PhotonNetwork.LocalPlayer.ActorNumber == killerNum)
        {
            Debug.Log("I am killer! I have to take pts...");
            if (killStreakRoutine == null)
            {
               killStreakCount = 1;
               killStreakRoutine = StartCoroutine(KillStreakRoutine());
            }
            else
            {
                UpdateKillStreakRoutine();
            }
        }
    }

    IEnumerator KillStreakRoutine()
    {
        if (killStreakCount == 1)
        {
            AddToPlayerProps(EXPFORSINGLEKILL);
        }
        else if (killStreakCount == 2)
        {
            AddToPlayerProps(EXPFORDOUBLEKILL);
        }
        else if(killStreakCount == 3)
        {
            AddToPlayerProps(EXPFORTRIPPLEKILL);
        }
        else if(killStreakCount == 4)
        {
            AddToPlayerProps(EXPFORULTRAKILL);
        }
        else if(killStreakCount >= 5)
        {
            AddToPlayerProps(EXPFORRAMPAGE);
        }

        yield return new WaitForSeconds(killStreakTime);
        killStreakCount = 0;
        killStreakRoutine = null;
    }

    void UpdateKillStreakRoutine()
    {
        StopCoroutine(killStreakRoutine);
        killStreakCount++;
        killStreakRoutine = StartCoroutine(KillStreakRoutine());
    }

    private void AddToPlayerProps(int amount)
    {
        var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;
        if(playerProps.ContainsKey("playerEXP"))
        {
            int overallExp = (int)playerProps["playerEXP"];
            overallExp += amount;

            playerProps.Remove("playerEXP");

            playerProps.Add("playerEXP", overallExp);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
        else
        {
            playerProps.Add("playerEXP", amount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
    }

    private void AddToRoomProps(int teamId, int amount)
    {
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (roomProps.ContainsKey("Team" + teamId.ToString()))
        {
            int overallExp = (int)roomProps["Team" + teamId.ToString()];
            overallExp += amount;

            roomProps.Remove("Team" + teamId.ToString());

            roomProps.Add("Team" + teamId.ToString(), overallExp);
            PhotonNetwork.LocalPlayer.SetCustomProperties(roomProps);
        }
        else
        {
            roomProps.Add("Team" + teamId.ToString(), amount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(roomProps);
        }
    }



    void AddBonusTeamEXP(int fromTeamID)
    {
        int teamPTS = PaintBallTeamManager.Instance.GetTeamPoints(fromTeamID) + 1; //TODO think about it...

        if(teamPTS == 1/*00*/)
        {
            AddToRoomProps(fromTeamID, EXP_FOR_100_TEAM_DMG);
        }
        else if(teamPTS == 5)
        {
            AddToRoomProps(fromTeamID, EXP_FOR_500_TEAM_DMG);
        }
        else if(teamPTS == 10)
        {
            AddToRoomProps(fromTeamID , EXP_FOR_1000_TEAM_DMG);
        }
        else if (teamPTS == 20)
        {
            AddToRoomProps(fromTeamID, EXP_FOR_2000_TEAM_DMG);
        }
        else if (teamPTS == 30)
        {
            AddToRoomProps(fromTeamID, EXP_FOR_3000_TEAM_DMG);
        }
    }
}
