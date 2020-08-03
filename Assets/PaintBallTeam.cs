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

    public PaintBallTeam(Color color, TEAM teamName, int playersMax)
    {
        this.color = color;
        this.teamName = teamName;
        this.playersMax = playersMax;
    }

    public void JoinTeam(PaintBallPlayer newPlayer)
    {
/*        for (int i = 0; i < playersInTeam.Count; i++)
        {
            if(playersInTeam[i] == null)
            {
                playersInTeam[i] = newPlayer;
                break;
            }
        }*/

        if(playersInTeam.Count < playersMax)
        {
            playersInTeam.Add(newPlayer);
            Debug.Log("PLAYER :" + newPlayer.nickName + "JOINED " + teamName.ToString() + " TEAM!");
        }
    }

}
