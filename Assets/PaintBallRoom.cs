using CMS.Config;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using SocialGTA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintBallRoom : MonoBehaviourPunCallbacks
{
/*    [SerializeField] string playerOrigin;*/
    const string roomPrefix = "Канал";

    [SerializeField]
    string lobbyName = "PaintBall";

    [SerializeField]
    byte maxPlayers = 8;

    public static GameManager instance;
    public MapConfig exitAfterDisconnect;
    bool _connectAndReady = false;

    [SerializeField] PaintBallTeamManager paintBallTeamManager;
    [SerializeField] StartPaintball startPaintball;

    public bool connected
    {
        get
        {
            return PhotonNetwork.InRoom && _connectAndReady;
        }
    }

    string gameVersion = "1";

    [SerializeField] List<RectTransform> playerInfos = new List<RectTransform>();
    [SerializeField] RectTransform playerInfoRect;
    [SerializeField] GameObject playerInfoPlaceHolder;

    private static Player myPlayer;

    private void Start()
    {
        Connect();
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom( "test" , new RoomOptions { MaxPlayers = maxPlayers} , TypedLobby.Default);

/*        Debug.Log("JOINED LOBBY!" + PhotonNetwork.CurrentLobby);*/
    }

    public override void OnConnectedToMaster()
    {
        JoinLobby();
    }

    private void JoinLobby()
    {

        TypedLobby lobbyType = new TypedLobby(lobbyName, LobbyType.Default);
        PhotonNetwork.JoinLobby(lobbyType);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers, CleanupCacheOnLeave = true });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PLAYERS IN ROOM! - " + PhotonNetwork.CurrentRoom.PlayerCount);

        SendLocalPlayerDataToMaster();
        SpawnExistingPlayers();
    }

    void SpawnExistingPlayers()
    {
        List<Player> connectedPlayers = PhotonNetwork.CurrentRoom.Players.Values.ToList();
        foreach (Player player in connectedPlayers)
        {
            OnPlayerPropertiesUpdate(player, player.CustomProperties);
        }
    }

    void SendLocalPlayerDataToMaster()
    {
        SaveManager saveManager = SaveManager.Instance;

        PaintBallPlayer newPlayerToJoin = new PaintBallPlayer(saveManager.LoadBody().ConfigId,
          saveManager.LoadClothesSet(saveManager.LoadBody().gender.ToString() + GameMode.Paintball.ToString()), saveManager.GetNickName());

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties.Add("newPlayerToJoin", JsonConvert.SerializeObject(newPlayerToJoin));
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
    }
    /// </remarks>
    public override void OnLeftRoom()
    {
        _connectAndReady = false;
        PhotonNetwork.LeaveLobby();
    }
/*    public override void OnDisconnected(DisconnectCause cause)
    {
        if (exitAfterDisconnect != null)
        {
          //  Loader.Instance.LoadGameScene(exitAfterDisconnect);

        }
    }*/

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps["newPlayerToJoin"] != null)
        {
            if (PhotonNetwork.IsMasterClient)
                SpawnNewPlayerForMaster(targetPlayer, changedProps);
        }
        if (changedProps["newPlayerForClients"] != null)
        {
            SpawnNewPlayerForClients(targetPlayer, changedProps);
        }
    }

    private void SpawnNewPlayerForClients(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps["newPlayerForClients"] != null && !paintBallTeamManager.PlayerIsInTeam(targetPlayer.ActorNumber.ToString()))
        {
            string playerJson = changedProps["newPlayerForClients"].ToString();
            PaintBallPlayer finalPlayer = JsonConvert.DeserializeObject<PaintBallPlayer>(playerJson);
            int teamIndex = (int)changedProps["teamIndex"];

            SpawnPlayer(finalPlayer, teamIndex);
        }
    }

    private void SpawnNewPlayerForMaster(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (paintBallTeamManager.LookForEmptySlot() && !paintBallTeamManager.PlayerIsInTeam(targetPlayer.ActorNumber.ToString())) //
        {
            string newPlayerJSON = changedProps["newPlayerToJoin"].ToString();
            PaintBallPlayer player = JsonConvert.DeserializeObject<PaintBallPlayer>(newPlayerJSON);

            int teamToJoinIndex = paintBallTeamManager.PickTeam().teamIndex;
            changedProps.Add("teamIndex", teamToJoinIndex);

            player.photonUserID = targetPlayer.ActorNumber.ToString(); //

            string playerJSON = JsonConvert.SerializeObject(player);
            changedProps.Add("newPlayerForClients", playerJSON);

            SpawnPlayer(player, teamToJoinIndex);
            targetPlayer.SetCustomProperties(changedProps);
        }
    }

    private void SpawnPlayer(PaintBallPlayer finalPlayer, int teamIndex)
    {
        var newTeam = paintBallTeamManager.AddPlayerToTeam(teamIndex, finalPlayer);

        var pedestal = finalPlayer.GetTeamPedestal(newTeam);
        pedestal.GetComponent<PedestalController>().SpawnPlayerAndInfo(finalPlayer);
        Debug.Log("Player spawned");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        paintBallTeamManager.RemovePlayerFromGame(otherPlayer.ActorNumber.ToString()); //
/*        startPaintball.ToggleStartButton(PhotonNetwork.CurrentRoom.PlayerCount);*/

        Debug.Log("PLAYERS IN ROOM! - " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public void OnClick_exitPaintball()
    {
        PhotonNetwork.Disconnect();
        Loader.Instance.LoadGameScene(exitAfterDisconnect);
    }
}
