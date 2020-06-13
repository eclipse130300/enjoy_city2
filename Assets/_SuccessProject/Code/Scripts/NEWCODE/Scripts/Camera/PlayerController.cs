using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SocialGTA;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
  
    private void Awake()
    {
      
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
      
        
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }
}
