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

    private int myTeamIndex;

    private void Start()
    {
        myTeamIndex = PaintBallTeamManager.Instance.myTeam.teamIndex;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PLAYER_DEATH)
        {
            object[] data = (object[])photonEvent.CustomData;
            int killerNum = (int)data[0];
            int[] damagers = (int[])data[1];

/*            Debug.Log("SOME PLAYER SEEMS TO BE DEAD... killer :" + killerNum + " damagersLength " + damagers.Length);*/

            CheckForAssist(damagers);
            CheckForKill(killerNum);
        }
        else if (eventCode == GameEvents.HIT_RECIEVED)
        {
                object[] data = (object[])photonEvent.CustomData;
/*                int actorNum = (int)data[0];
                int currentHP = (int)data[1];
                int dmgAmount = (int)data[2];*/
                int fromTeamId = (int)data[3];

            if (fromTeamId == myTeamIndex)
            {
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
                AddPlayerEXP(EXPFORASSIST);
            }
        }
    }

    private void CheckForKill(int killerNum)
     {
        if (PhotonNetwork.LocalPlayer.ActorNumber == killerNum)
        {
/*            Debug.Log("I am killer! I have to take pts...");*/
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
            AddPlayerEXP(EXPFORSINGLEKILL);
            Debug.Log("1 kill");
        }
        else if (killStreakCount == 2)
        {
            AddPlayerEXP(EXPFORDOUBLEKILL);
            Debug.Log("2 kill");
        }
        else if(killStreakCount == 3)
        {
            AddPlayerEXP(EXPFORTRIPPLEKILL);
            Debug.Log("3 kill");
        }
        else if(killStreakCount == 4)
        {
            AddPlayerEXP(EXPFORULTRAKILL);
            Debug.Log("4 kill");
        }
        else if(killStreakCount >= 5)
        {
            AddPlayerEXP(EXPFORRAMPAGE);
            Debug.Log("5 kill");
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

    private void AddPlayerEXP(int amount)
    {
        var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;
        if(playerProps.ContainsKey("playerEXP"))
        {
            int overallExp = (int)playerProps["playerEXP"];
            overallExp += amount;

            Debug.Log("PlayerEXP is now:" + overallExp);

            playerProps.Remove("playerEXP");

            playerProps.Add("playerEXP", overallExp);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
        else
        {
            playerProps.Add("playerEXP", amount);

            Debug.Log("Initialize player EPX with amount:" + amount);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
    }

    private void AddTeamEXP(int teamId, int amount)
    {
        var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;
        if (playerProps.ContainsKey("Team" + teamId.ToString()))
        {
            int overallExp = (int)playerProps["Team" + teamId.ToString()];
            overallExp += amount;

            playerProps.Remove("Team" + teamId.ToString());

            Debug.Log("Team " + teamId.ToString() + "exp is now :" + overallExp);

            playerProps.Add("Team" + teamId.ToString(), overallExp);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
        else
        {
            playerProps.Add("Team" + teamId.ToString(), amount);

            Debug.Log("I initialize Team " + teamId.ToString() + "exp with amount :" + amount);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
    }


    void AddBonusTeamEXP(int fromTeamID)
    {
        int teamPTS = PaintBallTeamManager.Instance.GetTeamPoints(fromTeamID) + 1; //TODO think about it...

        if(teamPTS == 100/*00*/)
        {
            AddTeamEXP(fromTeamID, EXP_FOR_100_TEAM_DMG);
 /*           Debug.Log("Team dmg :" + teamPTS);*/
        }
        else if(teamPTS == 500)
        {
            AddTeamEXP(fromTeamID, EXP_FOR_500_TEAM_DMG);
/*            Debug.Log("Team dmg :" + teamPTS);*/
        }
        else if(teamPTS == 1000)
        {
            AddTeamEXP(fromTeamID , EXP_FOR_1000_TEAM_DMG);
/*            Debug.Log("Team dmg :" + teamPTS);*/
        }
        else if (teamPTS == 2000)
        {
            AddTeamEXP(fromTeamID, EXP_FOR_2000_TEAM_DMG);
/*            Debug.Log("Team dmg :" + teamPTS);*/
        }
        else if (teamPTS == 3000)
        {
            AddTeamEXP(fromTeamID, EXP_FOR_3000_TEAM_DMG);
/*            Debug.Log("Team dmg :" + teamPTS);*/
        }
    }
}
