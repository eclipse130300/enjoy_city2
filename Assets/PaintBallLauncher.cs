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

public class PaintBallLauncher : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] string playerOrigin;
    const string roomPrefix = "Канал";

    [SerializeField]
    string lobbyName = "City";

    [SerializeField]
    byte maxPlayers = 30;

    static Player myPlayer;
    public static GameManager instance;
    public MapConfig exitAfterDisconnect;
    bool _connectAndReady = false;

    public bool connected
    {
        get
        {
            return PhotonNetwork.InRoom && _connectAndReady;
        }

    }

    string gameVersion = "1";

    Paintball_lobby_UI_controller paintball_Lobby_UI_Controller;

    [SerializeField] List<GameObject> pedestals = new List<GameObject>();
    [SerializeField] List<RectTransform> playerInfos = new List<RectTransform>();
    [SerializeField] RectTransform playerInfoRect;
    [SerializeField] GameObject playerInfoPlaceHolder;
    private const int teamsAmount = 2;
    [SerializeField] PaintBallTeam[] teams = new PaintBallTeam[teamsAmount];
    [SerializeField] Color[] teamColors = new Color[teamsAmount];
    [SerializeField] int maxPlayersInTeam = 4;

    PhotonView photon;

    private void Awake()
    {
/*        PhotonNetwork.AutomaticallySyncScene = false;*/


        paintball_Lobby_UI_Controller = FindObjectOfType<Paintball_lobby_UI_controller>();

        photon = GetComponent<PhotonView>();
    }

    void InitializeTeams()
    {
        var allTEams = Enum.GetValues(typeof(TEAM)).Cast<TEAM>().ToList();
        /*        int arrayIndex = 0;*/

        for (int i = 0; i < allTEams.Count; i++)
        {
            GameObject[] teamPedestals = new GameObject[maxPlayersInTeam];
            pedestals.CopyTo(maxPlayersInTeam * i, teamPedestals, 0, maxPlayersInTeam);


            teams[i] = new PaintBallTeam(teamColors[i], allTEams[i], maxPlayersInTeam, teamPedestals, i);
        }
    }


    /*    public IEnumerator tryConnect()
        {
            PhotonNetwork.ConnectUsingSettings();
            while (!connected)
            {
                yield return null;

            }
            yield return null;
            Debug.Log("connectedTOPHOTON!");
        }*/

    private void Start()
    {
        InitializeTeams();

        Connect();
    }

    public bool LookForEmptySlot()
    {
        int allPlayersConnected = 0;

        foreach (var team in teams)
        {
            allPlayersConnected += team.playersInTeam.Count;
        }

        return allPlayersConnected < teamsAmount * maxPlayersInTeam;
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

    public override void OnConnected()
    {
/*        JoinLobby();*/
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom( "test" , new RoomOptions { MaxPlayers = maxPlayers} , TypedLobby.Default);

        Debug.Log("JOINED LOBBY!" + PhotonNetwork.CurrentLobby);
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

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers }); /*RoomOptions ro = new RoomOptions; ro.*/
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PLAYERS IN ROOM! - " + PhotonNetwork.CurrentRoom.PlayerCount);

        SendLocalPlayerDataToMaster();
        // return;

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

/*        string newPlayerStr = JsonConvert.SerializeObject(newPlayerToJoin);
        photon.RPC("MasterSearch", RpcTarget.MasterClient, newPlayerStr);   // TODO Check if master has recieved this call*/

        Debug.Log("SENT DATA TO MASTER!");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
/*        JoinLobby();*/
    }
    /// </remarks>
    public override void OnLeftRoom()
    {
        _connectAndReady = false;
        PhotonNetwork.LeaveLobby();
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
    }


    public override void OnCreatedRoom()
    {
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        if (exitAfterDisconnect != null)
        {
          //  Loader.Instance.LoadGameScene(exitAfterDisconnect);

        }
    }

    public override void OnRegionListReceived(RegionHandler regionHandler)
    {

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
    }



    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    { 
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient && changedProps["newPlayerToJoin"] != null)
        {
            if (LookForEmptySlot() && !PlayerIsInTeam(targetPlayer.UserId))
            {
                string newPlayerJSON = changedProps["newPlayerToJoin"].ToString();
                PaintBallPlayer player = JsonConvert.DeserializeObject<PaintBallPlayer>(newPlayerJSON);

                int teamToJoinIndex = PickTeam().teamIndex;
                changedProps.Add("teamIndex", teamToJoinIndex);

                player.photonUserID = targetPlayer.UserId;

                string playerJSON = JsonConvert.SerializeObject(player);
                changedProps.Add("Player", playerJSON);

                /*                var dataStr = JsonConvert.SerializeObject(teamToJoinIndex, player);*/

                AddPlayerToTeamAndSpawn(teamToJoinIndex, player);
                targetPlayer.SetCustomProperties(changedProps);
            }

        }
        else
        {
            if (changedProps["Player"] != null && !PlayerIsInTeam(targetPlayer.UserId))
            {
                string playerJson = changedProps["Player"].ToString();
                PaintBallPlayer finalPlayer = JsonConvert.DeserializeObject<PaintBallPlayer>(playerJson);

                int teamIndex = (int)changedProps["teamIndex"];

                AddPlayerToTeamAndSpawn(teamIndex, finalPlayer);
            }
        }
    }

/*    [PunRPC]
    void MasterSearch(string newPlayerStr)
    {
        PaintBallPlayer newPlayerToJoin = JsonConvert.DeserializeObject<PaintBallPlayer>(newPlayerStr);

        if (LookForEmptySlot() && !PlayerIsInTeam(newPlayerToJoin.photonUserID))
        {
            int teamToJoinIndex = PickTeam().teamIndex;

            photon.RPC("AddPlayerToTeamAndSpawn", , teamToJoinIndex, newPlayerStr);
*//*            AddPlayerToTeamAndSpawn(teamToJoinIndex, newPlayerToJoin);*//*
        }
    }*/

    bool PlayerIsInTeam(string userID)
    {
        foreach (var team in teams)
        {
            foreach (PaintBallPlayer pl in team.playersInTeam)
            {
                if(pl.photonUserID == userID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void AddPlayerToTeamAndSpawn(int teamIndex, PaintBallPlayer player)
    {
/*        PaintBallPlayer player = JsonConvert.DeserializeObject<PaintBallPlayer>(playerStr);*/


        PaintBallTeam playerNewTeam = null;

        foreach (var team in teams)
        {
            if(team.teamIndex == teamIndex)
            {
                playerNewTeam = team;
                playerNewTeam.JoinTeam(player);
            }
        }

        if(playerNewTeam == null)
        {
            Debug.LogError("CannotFind team by index");
        }
        else
        { 
            var pedestal = player.GetTeamPedestal(playerNewTeam);

            pedestal.GetComponent<PedestalController>().SpawnPlayerAndInfo(player);
            Debug.Log("Player spawned");
        }
    }

    PaintBallTeam PickTeam(/*PaintBallPlayer player*/)
    {
        //we check every team
        int theLessPlayerNum = maxPlayersInTeam;
        PaintBallTeam teamToJoin = null;

        //if one team has less players than another
        foreach (PaintBallTeam team in teams)
        {
            if (team.playersInTeam.Count < theLessPlayerNum)
            {
                theLessPlayerNum = team.playersInTeam.Count;
                teamToJoin = team;
            }
        }
/*        //join team
        teamToJoin.JoinTeam(player);*/

        return teamToJoin;
    }

    public void OnEvent(EventData photonEvent)
    {
        //when they recieve event, they check their own teams list and and new player(in field list and screen) if necessary
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public override void OnWebRpcResponse(OperationResponse response)
    {
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }


}

/*[System.Serializable]
public class JoinData
{
    public int teamIndex;
    public PaintBallPlayer player;

    public JoinData(PaintBallPlayer player, int teamIndex)
    {
        this.player = player;
        this.teamIndex = teamIndex;
    }
}*/
