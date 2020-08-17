using CMS.Config;
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

    public PaintBallPlayer (string bodyConfigId, ClothesConfig clothesConfig, string nickName/*, string photonUserID*/)
    {
        this.bodyConfigID = bodyConfigId;
        this.clothesConfig = clothesConfig;
        this.nickName = nickName;
/*        this.photonUserID = photonUserID;*/
    }

    public GameObject GetTeamPedestal(PaintBallTeam team)
    {
        var myPedestal = team.teamPedestals[team.playersInTeam.Count - 1];

        return myPedestal;
    }
      
    public void SetTeam(PaintBallTeam team)
    {
        teamIndex = team.teamIndex;
        teamName = team.teamName;
    }
}
