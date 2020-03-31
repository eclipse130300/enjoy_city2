using UnityEngine;

namespace SocialGTA.Network {

    public class SpawnController : BoltSingletonPrefab<SpawnController> {

        [SerializeField] Transform[] spawnsPlayer;

        private int rndID;

        public Transform SaveSpawn {
            get {
                return spawnsPlayer[rndID];
            }
        }

        public Transform GetRandopPlayerPos() {
            rndID = Random.Range(0, spawnsPlayer.Length);
            return spawnsPlayer[rndID];
        }
    }
}
