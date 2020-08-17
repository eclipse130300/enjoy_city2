using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PaintBallTeam
{
    public string hexColor;

    public TEAM teamName;
    public int playersMax;
    public  List<PaintBallPlayer> playersInTeam = new List<PaintBallPlayer>();
    public GameObject[] teamPedestals;
    public int teamIndex;

    public PaintBallTeam(Color color, TEAM teamName, int playersMax, GameObject[] teamPedestals, int teamIndex)
    {
        this.hexColor = ColorUtility.ToHtmlStringRGB(color);

        this.teamName = teamName;
        this.playersMax = playersMax;
        this.teamPedestals = teamPedestals;
        this.teamIndex = teamIndex;
    }

    public Color GetTeamColor()
    {
        Color col;
        ColorUtility.TryParseHtmlString(hexColor, out col);
        return col;
    }

    public void JoinTeam(PaintBallPlayer newPlayer)
    {
        if (playersInTeam.Count < playersMax)
        {
            playersInTeam.Add(newPlayer);
/*            Debug.Log("PLAYER :" + newPlayer.nickName + "JOINED " + teamName.ToString() + " TEAM!");*/
        }
    }

    public void RemoveFromTeam(PaintBallPlayer player)
    {
        playersInTeam.Remove(player);
    }

}
