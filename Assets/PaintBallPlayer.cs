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
    public string photonUserID;
/*    public int playerId;*/

/*    public PaintBallTeam playerTeam;*/
/*    public GameObject pedestalUI;*/

    public PaintBallPlayer (string bodyConfigId, ClothesConfig clothesConfig, string nickName)
    {
        this.bodyConfigID = bodyConfigId;
        this.clothesConfig = clothesConfig;
        this.nickName = nickName;
    }

    public GameObject GetTeamPedestal(PaintBallTeam team)
    {
        var myPedestal = team.teamPedestals[team.playersInTeam.Count - 1];

/*        pedestalUI = myPedestal;*/

        return myPedestal;
    }
/*
    public static object Deserialize(byte[] data)
    {
        var result = new PaintBallPlayer(data[0], ()data[1],) ;
        result.Id = data[0];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        var c = (PaintBallPlayer)customType;
        return new byte[] { (byte)c.bodyConfigID };
    }*/
}
