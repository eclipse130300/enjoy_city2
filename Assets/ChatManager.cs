﻿using ExitGames.Client.Photon;
using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public RectTransform fullChatOverlay;

    [SerializeField] string playerID;

    private TouchScreenKeyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(ChatSettings.Load().AppId, "0.1", new AuthenticationValues(playerID));
    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();

        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done && keyboard.text != "")
            {
                string txt = keyboard.text;
                chatClient.PublishMessage("public", txt);
                keyboard.text = "";
            }
        }
        Debug.Log(fullChatOverlay.anchorMax);
        Debug.Log(fullChatOverlay.anchorMin);
    }

    public void OnSendButtonClick()
    {
        if (inputF.text != "")
        {
            chatClient.PublishMessage("public", inputF.text);
            inputF.text = "";
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
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        {
            string msgs = "";
            for (int i = 0; i < senders.Length; i++)
            {

                msgs = string.Format("{0}{1}: {2} ", msgs, senders[i], messages[i]);
                if (msgs.Contains("notification: "))
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
        closedChat.SetActive(true);
    }

    public void OnOpenFullChat()
    {
        closedChat.SetActive(false);
        fullSceenChat.SetActive(true);

        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

        //make chat text area fix keyboard size
        var kbarea = TouchScreenKeyboard.area;
        RectTransform canvas = fullChatOverlay.parent.GetComponent<RectTransform>();
        fullChatOverlay.pivot = new Vector2(1, 1);
        /*fullChatOverlay.anchorMin = new Vector2(0, kbarea.height).normalized;*/
        fullChatOverlay.anchorMax = new Vector2(canvas.rect.width, canvas.rect.height).normalized;
        fullChatOverlay.sizeDelta = new Vector2(canvas.rect.width, canvas.rect.height - kbarea.height);

        /*fullChatOverlay.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.rect.height - kbarea.height);*/
        fullChatOverlay.anchoredPosition = Vector2.zero;
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
