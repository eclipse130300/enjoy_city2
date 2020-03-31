using System.Collections.Generic;
using UnityEngine;
using Bolt;

namespace SocialGTA.Network {

    [BoltGlobalBehaviour(BoltNetworkModes.Server, "City_2")]
    public class NetworkCallbacks : GlobalEventListener {

        #region Variable

        List<string> logMessages = new List<string>();

        #endregion

        #region Standar Functions

        private void Awake () {
            PlayerObjectRegistry.CreateServerPlayer();
        }

        #endregion

        #region Photon Methods

        public override void Connected (BoltConnection connection) {
            PlayerObjectRegistry.CreateClientPlayer(connection);
        }

        public override void SceneLoadLocalDone(string scene) {
            var trans = SpawnController.instance.GetRandopPlayerPos();
            PlayerObjectRegistry.ServerPlayer.Spawn(trans.position);
        }

        public override void ControlOfEntityGained (BoltEntity entity) {
            PlayerCamera.instance.SetTarget(entity);
        }

        public override void SceneLoadRemoteDone (BoltConnection connection) {
            var trans = SpawnController.instance.SaveSpawn;
            PlayerObjectRegistry.GetTutorialPlayer(connection).Spawn(trans.position);
        }

        public override void OnEvent(LogEvent evnt) {
            logMessages.Add(evnt.Message);
            UpdateServerCanvasUI();
        }

        #endregion

        #region Custom Functions

        private void UpdateServerCanvasUI() {
            string message = "";
            int maxMssgs = Mathf.Min(5, logMessages.Count);

            for(int i = 0; i < maxMssgs; ++i)
                message += logMessages[i] + "/n";

            //logEventText.text = message;
        }

        #endregion

    }
}
