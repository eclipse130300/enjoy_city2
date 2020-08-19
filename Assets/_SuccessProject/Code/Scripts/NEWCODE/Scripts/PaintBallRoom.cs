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

    private PhotonView photon;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Connect();
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

        if(!PhotonNetwork.IsMasterClient)
        SpawnExistingPlayers();
    }

    void SpawnExistingPlayers()
    {
        List<Player> connectedPlayers = PhotonNetwork.CurrentRoom.Players.Values.ToList();
        foreach (Player player in connectedPlayers)
        {
            if (player != PhotonNetwork.LocalPlayer) //meaning spawn everybody except us (we are already spawned)
            {
                /*OnPlayerPropertiesUpdate(player, player.CustomProperties);*/
                //we take player custom props
                var playerProps = player.CustomProperties;
                //we deserialize player with team 
                string playerSTR = playerProps["playerWithTeam"] as string;

                // we callon player joins here???
                OnNewPlayerJoins(playerSTR);
            }
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
                PickTeamForNewPlayer(targetPlayer, changedProps);
        }
        /*        if (changedProps["newPlayerWithTeam"] != null)
                {
                    if(!PhotonNetwork.IsMasterClient)
                    SpawnNewPlayerForClients(targetPlayer, changedProps);
                }*/
    }

/*    private void SpawnNewPlayerForClients(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!paintBallTeamManager.PlayerIsInTeam(targetPlayer.ActorNumber*//*.ToString()*//*))
        {
            string playerJson = changedProps["newPlayerWithTeam"].ToString();
            PaintBallPlayer finalPlayer = JsonConvert.DeserializeObject<PaintBallPlayer>(playerJson);
            int teamIndex = (int)changedProps["teamIndex"];

            OnNewPlayerJoins(finalPlayer, teamIndex);
        }
    }*/

    private void PickTeamForNewPlayer(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (paintBallTeamManager.LookForEmptySlot() && !paintBallTeamManager.PlayerIsInTeam(targetPlayer.ActorNumber/*.ToString()*/)) //
        {
            string newPlayerJSON = changedProps["newPlayerToJoin"].ToString();
            PaintBallPlayer player = JsonConvert.DeserializeObject<PaintBallPlayer>(newPlayerJSON);

            player.SetTeam(paintBallTeamManager.PickTeam());


            //Master fills empty fields for new player
            player.photonActorNumber = targetPlayer.ActorNumber;

            string playerWithTeam = JsonConvert.SerializeObject(player);

            changedProps.Add("playerWithTeam", playerWithTeam);
            targetPlayer.SetCustomProperties(changedProps);
/*            SetTeamsToRoomProps();*/

            photon.RPC("OnNewPlayerJoins", RpcTarget.All, playerWithTeam);
        }
    }

/*    private void SetTeamsToRoomProps()
    {
        //add teams to room props for spawning players later
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if (roomProps.ContainsKey("teams"))
        {
            roomProps.Remove("teams");
        }
        string teamsSTR = JsonConvert.SerializeObject(roomProps);
        roomProps.Add("teams", teamsSTR);

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }*/


    [PunRPC]
    private void OnNewPlayerJoins(string playerWithTeam)
    {
        PaintBallPlayer player = JsonConvert.DeserializeObject<PaintBallPlayer>(playerWithTeam);
        var newTeam = paintBallTeamManager.AddPlayerToTeam(player.teamIndex, player);

        var pedestal = player.GetTeamPedestal(newTeam);

        var pedestalController = pedestal.GetComponent<PedestalController>();

        pedestalController.SpawnPlayerAndInfo(player);
        player.myPedestalIndex = pedestalController.pedestalID;

        //if the player who joined is ours, add this player as my player for future
        if (player.photonActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)  paintBallTeamManager.myPlayer = player;
    }

    [PunRPC]
    private void OnPlayerLeaves()
    {

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogError("Player left room - :" + otherPlayer.NickName);

        base.OnPlayerLeftRoom(otherPlayer);

        paintBallTeamManager.RemovePlayerFromGame(otherPlayer.ActorNumber);

/*        Debug.Log("PLAYERS IN ROOM! - " + PhotonNetwork.CurrentRoom.PlayerCount);*/
    }

    public void OnClick_exitPaintball()
    {
        PhotonNetwork.Disconnect();
        Loader.Instance.LoadGameScene(exitAfterDisconnect);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log(cause);
    }

}
