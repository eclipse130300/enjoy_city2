using Bolt;

namespace SocialGTA.Network {

    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public class ServerCallback : GlobalEventListener {

        #region Bolt Methods

        public override void Connected(BoltConnection connection) {
            var log = LogEvent.Create();
            log.Message = string.Format("{0} connected.", connection.RemoteEndPoint);
            log.Send();
        }

        public override void Disconnected(BoltConnection connection) {
            var log = LogEvent.Create();
            log.Message = string.Format("{0} disconnected.", connection.RemoteEndPoint);
            log.Send();
        }

        #endregion
    }
}
