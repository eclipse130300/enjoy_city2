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

    private void OnEnable()
    {
        //every time we load this script we
        Loader.Instance.AllSceneLoaded += SetButtonToUnreadyState;
    }

    private void OnDisable()
    {
        Loader.Instance.AllSceneLoaded -= SetButtonToUnreadyState;
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

    void SetButtonToUnreadyState()
    {
        playerIsReady = false;
        buttonImg.color = unReadyColor;
        PlayerIsReadyEvent(playerIsReady);
    }
}
