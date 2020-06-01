using ExitGames.Client.Photon;
using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    ChatClient chatClient;
    public TMP_InputField inputF;
    public GameObject content;
    public TextMeshProUGUI chatObjectText;
    public GameObject openedChat;
    public GameObject closedChat;
    [SerializeField] string playerID;

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
                chatObjectText.text += msgs + "\n";
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
            Debug.Log(ch);
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

    public void OnClosseChat()
    {
        openedChat.SetActive(false);
        closedChat.SetActive(true);
    }

    public void OnOpenChat()
    {
        closedChat.SetActive(false);
        openedChat.SetActive(true);
    }
}
