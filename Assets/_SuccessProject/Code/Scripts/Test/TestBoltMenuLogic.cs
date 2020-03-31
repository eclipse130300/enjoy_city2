using System;
using UdpKit;
using Bolt;
using Bolt.Matchmaking;

namespace SocialGTA.Network {

    public class TestBoltMenuLogic : GlobalEventListener {

        const string loadScene = "City_2";

        public void OnClickJoinServer() {
            BoltLauncher.StartClient();
        }

        public void OnClickStartServer() {
            BoltLauncher.StartServer();
        }
        
        public override void BoltStartDone() {
            if (BoltNetwork.IsServer) {
                string matchName = Guid.NewGuid().ToString();

                BoltMatchmaking.CreateSession(sessionID: matchName, sceneToLoad: loadScene);
            }
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList) {
            foreach(var session in sessionList) {
                UdpSession photonSession = session.Value as UdpSession;
                
                if(photonSession.Source == UdpSessionSource.Photon) {
                    BoltNetwork.Connect(photonSession);
                }
            }
        }
    }
}
