using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string playerOrigin;
    const string roomPrefix = "Канал";

    [SerializeField]
    string lobbyName = "City";
    [SerializeField]
    byte maxPlayers = 30;

    static Player myPlayer;
    public static GameManager instance;

    bool _connectAndReady = false;
    public bool connected {
        get {
           return PhotonNetwork.InRoom && _connectAndReady;
        }
        
    }

    private void Awake()
    {
        Loader.Instance.afterLoading = tryConnect();
    }
    public IEnumerator tryConnect() {
        PhotonNetwork.ConnectUsingSettings();
        while (!connected) {
            yield return null;
        
        }
        yield return null;
        Debug.Log("connected");
    }


    private void Start()
    {
       
    }
    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
        //JoinLobby();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        JoinLobby();
    }
    private void JoinLobby() {
       
        TypedLobby lobbyType = new TypedLobby(lobbyName, LobbyType.Default);
        PhotonNetwork.JoinLobby(lobbyType);
    }
    private void CreatePlayer()
    {
        Debug.Log("CreatePlayer");
        EntryPoint[] points = FindObjectsOfType<EntryPoint>();
        Debug.Log("CreatePlayer "+ points.Length);
        EntryPoint point = points.FirstOrDefault((x)=>{ 
            Debug.Log("x.mapCFG.ConfigId " + x.mapCFG.ConfigId + " priveousScene " + Loader.Instance.priveousScene.ConfigId); 
            return x.mapCFG.ConfigId == Loader.Instance.priveousScene.ConfigId;  
            });
        
        PhotonNetwork.Instantiate(playerOrigin, point.transform.position, point.transform.rotation);
        _connectAndReady = true;

      //  TypedLobby lobbyType = new TypedLobby(lobbyName, LobbyType.Default);
      // PhotonNetwork.JoinLobby(lobbyType);

    }
    public override void OnJoinedLobby()
    {
       

      
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.SuppressRoomEvents = false;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.PublishUserId = true;
      
        string roomName = "";
        foreach(var room in roomList)
        {
            if (room.PlayerCount < room.MaxPlayers) {
                Debug.Log("ROOM " + room.Name);
                roomName = room.Name;
            }
        }
        if (string.IsNullOrEmpty(roomName)) {
            roomName = roomPrefix + "_" + roomList.Count + 1;
        }
        
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
  
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed");
        PhotonNetwork.LeaveLobby();
    }
    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
        JoinLobby();
    }
    public override void OnJoinedRoom()
    {

        CreatePlayer();

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
    }

 

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
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

    /// <summary>
    /// Called when the client receives an event from the server indicating that an error happened there.
    /// </summary>
    /// <remarks>
    /// In most cases this could be either:
    /// 1. an error from webhooks plugin (if HasErrorInfo is enabled), read more here:
    /// https://doc.photonengine.com/en-us/realtime/current/gameplay/web-extensions/webhooks#options
    /// 2. an error sent from a custom server plugin via PluginHost.BroadcastErrorInfoEvent, see example here: 
    /// https://doc.photonengine.com/en-us/server/current/plugins/manual#handling_http_response
    /// 3. an error sent from the server, for example, when the limit of cached events has been exceeded in the room
    /// (all clients will be disconnected and the room will be closed in this case)
    /// read more here: https://doc.photonengine.com/en-us/realtime/current/gameplay/cached-events#special_considerations
    /// </remarks>
    /// <param name="errorInfo">object containing information about the error</param>
    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
    }
}
