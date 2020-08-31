using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int MaxHp;
    [SerializeField] private int currentHP;
    [SerializeField] int spawnSecounds;

    private PlayerTeam teamScript;
    PaintBallPlayerManipulator manipulator;
    Animator animator;


    private PhotonView photon;

    public bool isInvulnerable = false;
    [SerializeField] TextMesh worldTimer;

    [Header("kill streak CD system. Add 2nd to this gameObject!")]
    [SerializeField] CoolDownSystem cdSystem;

    [Header("How much time we save who damaged us(for kill streaks)")]
    [SerializeField] float killStreakCDTime = 10f;

    //test!!!!!!!
    public static int staticMaxHP;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();
        manipulator = GetComponent<PaintBallPlayerManipulator>();
        animator = GetComponent<Animator>();
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

    public void TakeDamage(int amount, int fromTeamIndex, int damagerNum)
    {
        if (photon == null) return; //meaning we shoot not a real player and it can't take damage(wall for ex.) for testing..

        currentHP -= amount;
        OnHitRecievedEvent(amount, fromTeamIndex, damagerNum);
        DeathCheck(damagerNum);
    }

    void DeathCheck(int potentialKillerNum)
    {
        if (currentHP <= 0)
        {
            isInvulnerable = true;

            OnDeathEvent(potentialKillerNum); //this is killer for other managers (data)

            double respawnTime = PhotonNetwork.Time + spawnSecounds;

            photon.RPC("DeathPlayerSequence", RpcTarget.AllViaServer, respawnTime); //this is for vfx player logic
        }
    }

    private void OnHitRecievedEvent(int dmgAmount, int fromTeamID, int damagerNum)
    {
        photon.RPC("SaveDamager", RpcTarget.AllViaServer, damagerNum);

        int actorNumber = 0;

        if (photon.Owner != null)
        {
            actorNumber = photon.Owner.ActorNumber; //who was damaged via photon
        }
        else
        {
            actorNumber = gameObject.GetInstanceID(); //if we spawn enemy manually
        }

        object[] content = new object[] { actorNumber, currentHP, dmgAmount, fromTeamID, damagerNum };  //TODO optimize it...send byte instead of int??
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.HIT_RECIEVED, content, raiseEventOptions, SendOptions.SendReliable);
    }

    [PunRPC]
    private void DeathPlayerSequence(double respawnTime)
    {
        //play death animation
        animator.SetLayerWeight(2, 0f);
        animator.SetTrigger("isDead");

        //we disable shooting in dead player
        isInvulnerable = true;
        //let's disable components we don't need during respawn routine
        manipulator.DisablePlayer();


        //start respawn CD
        StartCoroutine(RespawnRoutine(respawnTime));
    }

    private void OnDeathEvent(int killerNum)
    {
        //we clear damagers list from killer
        foreach (CoolDownData coolDown in cdSystem.coolDowns.ToList())
        {
            if (coolDown.Id == killerNum) cdSystem.coolDowns.Remove(coolDown);
        }

        //array of damagers who damaged dying player (except killer)
        int[] damagers = new int[cdSystem.coolDowns.Count];

        for (int i = 0; i < damagers.Length; i++)
        {
            damagers[i] = cdSystem.coolDowns[i].Id;
        }
    
        object[] content = new object[] {killerNum, damagers};  //TODO optimize it...send byte instead of int? - to do this we have to use PlayerNumbering...
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.PLAYER_DEATH, content, raiseEventOptions, SendOptions.SendReliable);
    }

    IEnumerator RespawnRoutine(double timeToStop)
    {
        worldTimer.gameObject.SetActive(true);

        while (PhotonNetwork.Time < timeToStop)
        {
            double timeLeft = timeToStop - PhotonNetwork.Time;

            var roundedTime = Mathf.RoundToInt((float)timeLeft);

            worldTimer.text = roundedTime.ToString();

            yield return null;
        }
        worldTimer.gameObject.SetActive(false);
        RecoverHP();

        //let's play resp animation
        animator.SetTrigger("isAlive");
        animator.SetLayerWeight(2, 1f);

        if (photon.IsMine) //if it's ours photon, let's update UI
        {
            PlayerRespawnedEvent();
            //at the end set animator to normal state, endable shooting, find spawnPoint and put player there
            PaintBallGameSpawner.Instance.RespawnPlayer(gameObject, teamScript.currentTeam);
        }

        yield return new WaitForSeconds(1f);  //let's wait a little longer to prevent async shooting


        manipulator.EnablePlayer();
        isInvulnerable = false;
    }

    void PlayerRespawnedEvent()
    {
        if (photon.Owner != null)
        {
            object[] content = new object[] { currentHP };

            int[] actorsTosend = new int[] { photon.Owner.ActorNumber };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = actorsTosend };
            PhotonNetwork.RaiseEvent(GameEvents.PLAYER_RESPAWNED, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    [PunRPC]
    void SaveDamager(int damagerNum)
    {
        CoolDownData newDamagerData = new CoolDownData(damagerNum, killStreakCDTime);

        foreach (CoolDownData cd in cdSystem.coolDowns.ToList())
        {
            if(cd.Id == newDamagerData.Id)
            {
                cdSystem.coolDowns.Remove(cd);
            }
        }
        cdSystem.PutOnCooldown(newDamagerData);
    }

}
