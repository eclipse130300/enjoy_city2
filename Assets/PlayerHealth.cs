using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerHealth : MonoBehaviour/*, IPunObservable*/
{

    [SerializeField] int MaxHp;
    [SerializeField] private int currentHP;

    private PlayerTeam teamScript;

    //test!!!!!!!
    public static int staticMaxHP;

    private void Awake()
    {
        //test!!! todo smthing with it!
        staticMaxHP = MaxHp;
        teamScript = GetComponent<PlayerTeam>();
    }

    private void OnEnable()
    {
        RecoverHP();
    }

    void RecoverHP()
    {
        currentHP = MaxHp;
    }

/*    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        DeathCheck();
    }*/

    public void TakeDamage(int amount, int fromTeamIndex)
    {
        currentHP -= amount;
        OnHitRecievedEvent(amount, fromTeamIndex);
        DeathCheck();
    }

    void DeathCheck()
    {
        if (currentHP <= 0)
        {
            //die function
            Debug.Log("I am dead!");
        }
    }

    private void OnHitRecievedEvent(int dmgAmount, int fromTeamID)
    {
        var actorNumber = PhotonNetwork.LocalPlayer.ActorNumber; //who was damaged


        object[] content = new object[] { actorNumber, currentHP, dmgAmount, fromTeamID };  //TODO optimize it...send byte instead of int??
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.HIT_RECIEVED, content, raiseEventOptions, SendOptions.SendReliable);
    }

/*    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHP);
        }
        else
        {
            currentHP = (int)stream.ReceiveNext();
        }
    }*/


}
