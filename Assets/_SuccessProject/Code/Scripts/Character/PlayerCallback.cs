using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialGTA.Network {

    [BoltGlobalBehaviour("City_2")]
    public class PlayerCallback : Bolt.GlobalEventListener {

        public override void SceneLoadLocalDone(string map) {
            /*PlayerCamera.Instantiate();*/
            SpawnController.Instantiate();
            GameCanvasController.Instantiate();
        }

        public override void ControlOfEntityGained(BoltEntity entity) {
            /*PlayerCamera.instance.SetTarget(entity);*/
        }
    }
}
