using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;

public class PaintBallPlayerInitializer : MonoBehaviour, IPunInstantiateMagicCallback
{
    private PaintBallTeamManager paintBallTeamManager;

    private void Awake()
    {
        paintBallTeamManager = FindObjectOfType<PaintBallTeamManager>();
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

/*        GameObject spawnedPlayerGO = (GameObject)info.Sender.TagObject;*/
        
    }

    void InitializeSpawnedPlayer(GameObject player, PaintBallPlayer paintBallPlayer)
    {
        //in skins manager change gameMode to paintball 
        player.GetComponent<SkinsManager>()._gameMode = GameMode.Paintball;

        PlayerTeam playerTeam = player.GetComponent<PlayerTeam>();

        var myTeam = paintBallTeamManager.GetTeamByIndex(paintBallPlayer.teamIndex);
        playerTeam.InitializePlayerTeam(paintBallPlayer, myTeam.hexColor);

        var playerShooting = player.GetComponent<ShootAbility>();
        playerShooting.InitializeShooting();
    }
}
