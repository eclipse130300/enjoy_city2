using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ReadyButton : MonoBehaviour
{
    bool playerIsReady = false;
    private Image buttonImg;

    [SerializeField] Color readyColor;
    [SerializeField] Color unReadyColor;

    private void Awake()
    {
        buttonImg = GetComponent<Image>();
    }

    private void Start()
    {
        buttonImg.color = unReadyColor;
    }

    public void OnClick()
    {
        playerIsReady = !playerIsReady;

        Color color = playerIsReady == true ? readyColor : unReadyColor;
        buttonImg.color = color;

        PlayerIsReadyEvent(playerIsReady);
    }

    void PlayerIsReadyEvent(bool value)
    {
            object[] content = new object[] { value };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(GameEvents.PLAYER_IS_READY_PAINTBALL_LOBBY, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
