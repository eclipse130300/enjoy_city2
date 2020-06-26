using ExitGames.Client.Photon;
using Photon.Chat;
using SocialGTA;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    public TMP_InputField inputF;
    public GameObject content;
    public TextMeshProUGUI [] chatObjectsText;
    public GameObject previewChat;
    public GameObject closedChat;
    public GameObject fullSceenChat;
    public GameObject closeFullChatButton;

    public RectTransform chatBoxToResize;

    [SerializeField] string playerID;

    private TouchScreenKeyboard keyboard;
    private RectTransform canvas;
    public TextMeshProUGUI placeholderchatTXT;

    public UnityEvent onEndEdit;
    // Start is called before the first frame update
    void Start()
    {
        AutorizationController autorization = new AutorizationController();
        autorization.Login();
        playerID = autorization.profile.UserName;
        chatClient = new ChatClient(this);
        chatClient.Connect(ChatSettings.Load().AppId, "0.1", new AuthenticationValues(playerID));

        canvas = chatBoxToResize.parent.GetComponent<RectTransform>();
/*        keyboard = FindObjectOfType<TMP_InputField>();*/

    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();
        TouchScreenKeyboard.hideInput = true;

        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done && keyboard.text != "")
            {
                string txt = keyboard.text;
                chatClient.PublishMessage("public", txt);
                keyboard.text = "";
            }
        }

       // MobileDebug.Log("TouchScreenKeyboard "+ keyboard + " status " + keyboard?.status + " ratio " +MobileUtilities.GetKeyboardHeightRatio(true), "Chat",LogType.Log,1);
        MobileDebug.Log("TouchScreenKeyboard.visible "+TouchScreenKeyboard.visible, "Chat", LogType.Log, 1);
        MobileDebug.Log("TouchScreenKeyboard.hideInput " + TouchScreenKeyboard.hideInput, "Chat", LogType.Log,2);
        MobileDebug.Log("TouchScreenKeyboard.isInPlaceEditingAllowed " + TouchScreenKeyboard.isInPlaceEditingAllowed, "Chat", LogType.Log, 3);
        MobileDebug.Log("TouchScreenKeyboard.area " + TouchScreenKeyboard.area, "Chat", LogType.Log, 4);
        MobileDebug.Log("TouchScreenKeyboard.isSupported " + TouchScreenKeyboard.isSupported, "Chat", LogType.Log, 5);
        MobileDebug.Log("MobileUtilities.GetKeyboardHeightRatio(true) " + MobileUtilities.GetKeyboardHeightRatio(true), "Chat", LogType.Log, 6);
        MobileDebug.Log("MobileUtilities.GetKeyboardHeightRatio(false) " + MobileUtilities.GetKeyboardHeightRatio(false), "Chat", LogType.Log, 7);
        MobileDebug.Log("MobileUtilities.GetKeyboardHeight(true) " + MobileUtilities.GetKeyboardHeight(true), "Chat", LogType.Log, 8);
        MobileDebug.Log("MobileUtilities.GetKeyboardHeight(false) " + MobileUtilities.GetKeyboardHeight(false), "Chat", LogType.Log, 9);
        MobileDebug.Log("Screen.width " + Screen.width, "Chat", LogType.Log, 10);
        MobileDebug.Log("Screen.height " + Screen.height, "Chat", LogType.Log, 11);


        /*if (keyboard?.status == TouchScreenKeyboard.Status.Visible)*/
        if (TouchScreenKeyboard.visible)
        {
            //make chat text area fix keyboard size


#if !UNITY_EDITOR

            float KBheight = MobileUtilities.GetKeyboardHeight(false);
            float CanvasToScreenRatio = (float)Screen.height / canvas.rect.height;
            float chatSize = canvas.rect.height - (KBheight / CanvasToScreenRatio);

            chatBoxToResize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chatSize);
            chatBoxToResize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.rect.width);

            chatBoxToResize.anchoredPosition = new Vector2(0, 0);


            MobileDebug.Log("KBheight " + KBheight, "Chat", LogType.Log, 12);
#else


            chatBoxToResize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.rect.width);
            chatBoxToResize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.rect.height);
#endif
        }
        else
        {

            chatBoxToResize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.rect.width);
            chatBoxToResize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.rect.height);

        }


    }

    private void SetChatSize(RectTransform rect, float Vsize)
    {
    }


    public void OnSendButtonClick()
    {
        if (inputF.text != "")
        {
            chatClient.PublishMessage("public", inputF.text);
            inputF.text = "";
        }
        else
        {
            chatClient.PublishMessage("public", placeholderchatTXT.text);
            placeholderchatTXT.text = "";
        }
    }

    /* void PrintMessageToChat*/

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state);
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { "public" });

    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        {
            string msgs = "";
            for (int i = 0; i < senders.Length; i++)
            {

                msgs = string.Format("{0}{1}: {2} ", msgs, senders[i], messages[i]);
                if (msgs.Contains("notification :"))
                {
                    msgs = (string)messages[i];
                }
                foreach (TextMeshProUGUI text in chatObjectsText)
                {
                    text.text += msgs + "\n";
                }

                if (senders[i] != playerID && fullSceenChat.activeInHierarchy == false )  ShowPreviewChat();
            }
            Debug.Log(string.Format("OnGetMessages: {0} ({1}) > {2}", channelName, senders.Length, msgs));

        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach(string ch in channels)
        {
            string str = "notification :" + "игрок " + playerID + " подключился к каналу " + ch;
            chatClient.PublishMessage("public", str);
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnCloseFullChat()
    {
        fullSceenChat.SetActive(false);
        closeFullChatButton.SetActive(false);

        closedChat.SetActive(true);
    }

    public void OnOpenFullChat()
    {
        fullSceenChat.SetActive(true);
        closeFullChatButton.SetActive(true);

        closedChat.SetActive(false);
/*
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);*/
    }

    public void ShowPreviewChat()
    {
        previewChat.SetActive(true);
        Invoke("HidePreviewChat", 3f); //todo coroutine
    }

    public void HidePreviewChat()
    {
        previewChat.SetActive(false);
    }
}
