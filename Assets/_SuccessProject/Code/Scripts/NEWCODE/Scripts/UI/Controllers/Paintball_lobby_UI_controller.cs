using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Paintball_lobby_UI_controller : MonoBehaviourPunCallbacks, IPunObservable
{




    private void Start()
    {
/*        InitializeTeams();*/
    }



    public void OnPlayerDisconected()
    {

    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }
}