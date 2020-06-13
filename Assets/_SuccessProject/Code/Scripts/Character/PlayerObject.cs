using UnityEngine;

namespace SocialGTA.Network {
    /*
    public class PlayerObject {

        public BoltEntity character;
        public BoltConnection connection;

        public bool IsServer {
            get { return connection == null; }
        }

        public bool IsClient {
            get { return connection != null; }
        }

        public void Spawn (Vector3 pos) {
            if (!character) {
                character = BoltNetwork.Instantiate(BoltPrefabs.Player, pos, Quaternion.identity);

                if (IsServer) {
                    character.TakeControl();
                } else {
                    character.AssignControl(connection);
                }
            }

            character.transform.position = pos;
        }
    }*/
}
