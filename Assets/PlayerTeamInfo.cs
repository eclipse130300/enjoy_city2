using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerTeamInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nickName;
    [SerializeField] Image bgImage;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

    }

    public void Initialize(string nick, Color teamColor)
    {
        nickName.text = nick;

        Color col = teamColor;
        //let's make info panel transparent a little
        col.a = 0.5f;
        bgImage.color = col;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            //local stuff
            GameObject player = (GameObject)PhotonNetwork.LocalPlayer.TagObject;
            GameObject playerCam = player.GetComponentInChildren<PlayerCamera>().gameObject;
            canvas.worldCamera = playerCam.GetComponent<Camera>();
        }
    }
}
