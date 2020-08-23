using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PaintBallPlayerManipulator : MonoBehaviour, IPunInstantiateMagicCallback, IOnEventCallback
{
    ThirdPersonInput input;
    ShootAbility shootAbility;

    private void Awake()
    {
        input = GetComponent<ThirdPersonInput>();
        shootAbility = GetComponent<ShootAbility>();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var customProps = info.Sender.CustomProperties;
        if (customProps["playerWithTeam"] != null)
        {
            string newPlayer = customProps["playerWithTeam"].ToString();
            PaintBallPlayer spawnedPlayer = JsonConvert.DeserializeObject<PaintBallPlayer>(newPlayer);
            InitializeSpawnedPlayer(gameObject, spawnedPlayer);
        }
        else
        {
            Debug.LogError("Dont have key [playerWithTeam] in custom props");
        }
    }

    void InitializeSpawnedPlayer(GameObject player, PaintBallPlayer paintBallPlayer)
    {
        //in skins manager change gameMode to paintball 
        player.GetComponent<SkinsManager>()._gameMode = GameMode.Paintball;

        PlayerTeam playerTeam = player.GetComponent<PlayerTeam>();
        var myTeam = PaintBallTeamManager.Instance.GetTeamByIndex(paintBallPlayer.teamIndex);

        Color teamColor;
        ColorUtility.TryParseHtmlString("#" + myTeam.hexColor, out teamColor);


        playerTeam.InitializePlayerTeam(myTeam, teamColor);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PAINTBALL_GAME_FINISHED) //just turn off components to disable our player 
        {
            DisablePlayer();
        }
    }

    public void DisablePlayer()
    {
        input.enabled = false;
        shootAbility.enabled = false;
    }

    public void EnablePlayer()
    {
        input.enabled = true;
        shootAbility.enabled = true;
    }
}
