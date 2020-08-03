using CMS.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PaintBallPlayer
{
    public BodyConfig bodyConfig;
    public ClothesConfig clothesConfig;
    public string nickName;

    public PaintBallTeam playerTeam;
    public GameObject pedestalUI;

    public PaintBallPlayer (BodyConfig bodyConfig, ClothesConfig clothesConfig, string nickName)
    {
        this.bodyConfig = bodyConfig;
        this.clothesConfig = clothesConfig;
        this.nickName = nickName;
    }

    public GameObject GetTeamPedestal(PaintBallTeam team)
    {
        var myPedestal = team.teamPedestals[team.playersInTeam.Count - 1];

        pedestalUI = myPedestal;

        return pedestalUI;
    }
}
