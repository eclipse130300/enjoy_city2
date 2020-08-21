using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int MaxHp;
    [SerializeField] private int currentHP;
    [SerializeField] int spawnSecounds;

    private PlayerTeam teamScript;
    private ThirdPersonInput input;
    private ShootAbility shooting;


    private PhotonView photon;

    public bool isInvulnerable = false;
    [SerializeField] TextMesh worldTimer;

    //test!!!!!!!
    public static int staticMaxHP;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();

        //test!!! todo smthing with it!
        staticMaxHP = MaxHp;

        teamScript = GetComponent<PlayerTeam>();
        input = GetComponent<ThirdPersonInput>();
        shooting = GetComponent<ShootAbility>();
    }

    private void OnEnable()
    {
        RecoverHP();
    }

    void RecoverHP()
    {
        currentHP = MaxHp;
    }

    public void TakeDamage(int amount, int fromTeamIndex)
    {
        if (photon == null) return; //meaning we shoot not a real player and it can't take damage(wall for ex.) for testing..

        currentHP -= amount;
        OnHitRecievedEvent(amount, fromTeamIndex);
        DeathCheck();
    }

    void DeathCheck()
    {
        if (currentHP <= 0)
        {
            isInvulnerable = true;
            //die function
            photon.RPC("DeathSecuence", RpcTarget.AllViaServer);
        }
    }

    private void OnHitRecievedEvent(int dmgAmount, int fromTeamID)
    {

        var actorNumber = photon.Owner.ActorNumber; //who was damaged


        object[] content = new object[] { actorNumber, currentHP, dmgAmount, fromTeamID };  //TODO optimize it...send byte instead of int??
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.HIT_RECIEVED, content, raiseEventOptions, SendOptions.SendReliable);
    }

    [PunRPC]
    private void DeathSecuence()
    {
        //play death animation
        Debug.Log("I am dead!Death animation has to be here...");
        //we disable shooting in dead player
        isInvulnerable = true;
        //let's disable components we don't need during respawn routine
        input.enabled = false;
        shooting.enabled = false;

        //start respawn CD
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        worldTimer.gameObject.SetActive(true);

        int currentTime = spawnSecounds;
        for (int i = 0; i < spawnSecounds; i++)
        {
            worldTimer.text = currentTime.ToString();

            currentTime--;
            yield return new WaitForSeconds(1f);
        }
        worldTimer.gameObject.SetActive(false);

        PaintBallGameSpawner.Instance.RespawnPlayer(gameObject, teamScript.currentTeam);

        //at the end set animator to normal state, endable shooting, find spawnPoint and put player there
        isInvulnerable = false;
        //let's disable components we don't need during respawn routine
        input.enabled = true;
        shooting.enabled = true;


        RecoverHP();
    }

}
