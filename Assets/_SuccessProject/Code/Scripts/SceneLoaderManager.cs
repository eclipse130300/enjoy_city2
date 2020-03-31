using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Bolt;
using UdpKit;
using Bolt.Matchmaking;

public class SceneLoaderManager : GlobalEventListener {

    #region Vairables

    public static SceneLoaderManager Instance;

    [Header("Components")]
    [SerializeField] GameObject loadingCanvas;

    const int MAINMENU_ID = 2;
    private string _serverMapName;

    #endregion

    #region Standart Functions

    private void Awake () {
        Instance = this;
    }

    #endregion

    #region Cunstom Functions

    public void LoadAsyncScene (int sceneID) {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    private IEnumerator LoadSceneAsync (int sceneID) {
        loadingCanvas.SetActive(true);
        yield return null;

        AsyncOperation laod = SceneManager.LoadSceneAsync(sceneID);

        while (true) {
            if (laod.isDone) {
                loadingCanvas.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }

    public void StartServer(string mapName) {
        _serverMapName = mapName;
        BoltLauncher.StartServer();
    }

    public void ConnectionInServer () {
        BoltLauncher.StartClient();
    }

    public void DiscconnectFromServer () {
        StartCoroutine(DisconnecterFromServer());
    }

    private IEnumerator DisconnecterFromServer () {
        loadingCanvas.gameObject.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(MAINMENU_ID);

        while (true) {
            if (loadSceneAsync.isDone) {
                loadingCanvas.gameObject.SetActive(false);

                yield break;
            }
        }
    }

    #region Bolt Functions

    public override void BoltStartBegin () {
        loadingCanvas.SetActive(true);
    }

    public override void BoltStartDone () {
        if (BoltNetwork.IsServer) {
            string matchName = Guid.NewGuid().ToString();

            BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: _serverMapName);
        }

        loadingCanvas.SetActive(false);
    }

    public override void BoltStartFailed () {
        loadingCanvas.SetActive(false);
    }

    public override void SessionListUpdated (Map<Guid, UdpSession> sessionList) {
        foreach (var session in sessionList) {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon) {
                BoltNetwork.Connect(photonSession);
            }
        }
    }

    #endregion

    #endregion

}
