﻿using CMS.Config;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PaintBallPlayer
{
    public string bodyConfigID;
    public ClothesConfig clothesConfig;
    public string nickName;
    public int photonActorNumber;
    public TEAM teamName;
    public int teamIndex;
    public int myPedestalIndex;

    public PaintBallPlayer (string bodyConfigId, ClothesConfig clothesConfig, string nickName/*, string photonUserID*/)
    {
        this.bodyConfigID = bodyConfigId;
        this.clothesConfig = clothesConfig;
        this.nickName = nickName;
/*        this.photonUserID = photonUserID;*/
    }

    public GameObject GetTeamPedestal(PaintBallTeam team)
    {
        //we take empty team pedestal
        var myPedestal = team.teamPedestals[team.playersInTeam.Count - 1];

        // we fill pedestalID for future spawn point
        myPedestalIndex = myPedestal.GetComponent<PedestalController>().pedestalID;

        return myPedestal;
    }
      
    public void SetTeam(PaintBallTeam team)
    {
        teamIndex = team.teamIndex;
        teamName = team.teamName;
    }
}
