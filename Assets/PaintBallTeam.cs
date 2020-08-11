using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PaintBallTeam
{
    public Color color;
    public TEAM teamName;
    public int playersMax;
    public  List<PaintBallPlayer> playersInTeam = new List<PaintBallPlayer>();
    public GameObject[] teamPedestals;
    public int teamIndex;

    public PaintBallTeam(Color color, TEAM teamName, int playersMax, GameObject[] teamPedestals, int teamIndex)
    {
        this.color = color;
        this.teamName = teamName;
        this.playersMax = playersMax;
        this.teamPedestals = teamPedestals;
        this.teamIndex = teamIndex;
    }

    public void JoinTeam(PaintBallPlayer newPlayer)
    {

        if(playersInTeam.Count < playersMax)
        {
            playersInTeam.Add(newPlayer);
/*            Debug.Log("PLAYER :" + newPlayer.nickName + "JOINED " + teamName.ToString() + " TEAM!");*/
        }
    }

}
