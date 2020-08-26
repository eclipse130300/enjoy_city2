using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PaintBallPointsPanel : MonoBehaviour
{
    public TextMeshProUGUI expTXT;
    public TextMeshProUGUI softTXT;

    public void SetResult(int finalEXP, int finalSoft)
    {
        expTXT.text = "+"+ finalEXP.ToString();
        softTXT.text = "+"+ finalSoft.ToString();
    }

    private void OnDisable()
    {
        //as soon as this panel dissapears, we can and this game and start one more game
        if(PhotonNetwork.IsMasterClient)
        {
            GameResultsFinishedEvent();
        }
    }

    void GameResultsFinishedEvent()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.PAINTBALL_FINISHED, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
