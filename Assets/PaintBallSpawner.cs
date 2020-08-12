using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallSpawner : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        //instantinate player
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //in skins manager change gameMode to paintball 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
