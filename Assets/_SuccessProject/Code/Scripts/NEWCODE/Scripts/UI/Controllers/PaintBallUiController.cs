using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintBallUiController : MonoBehaviour, IOnEventCallback
{
    /*    [SerializeField] FixedButton shootButton;*/
    [Header("Interaction_Buttons")]
    [SerializeField] FixedButton reloadButton;
    [SerializeField] FixedButton SuperShotButton;
    [SerializeField] FixedButton powerUpButton;

    [Header("Crosshair")]
    [SerializeField] Image ammoFill;
    [SerializeField] float lerpSpeed;
    [SerializeField] RectTransform crosshairRect;

    [Header("Teams")]
    [SerializeField] Image redTeamFill;
    [SerializeField] TextMeshProUGUI redTeamScore;
    [SerializeField] Image blueTeamFill;
    [SerializeField] TextMeshProUGUI blueTeamScore;

    [Header("Player")]
    [SerializeField] Image playerHPfill;
    [SerializeField] TextMeshProUGUI playerHPamount;

    [Header("GameTimer")]
    [SerializeField] TextMeshProUGUI gameTimer;

    [Header("Result text")]
    [SerializeField] TextMeshProUGUI resultText;

    public TextMeshProUGUI startCD;
    public GameObject playerCamera;
    public LayerMask noPlayerLayerMask;

    private PaintBallTeamManager paintballTM;

    //test
    Vector3 shotPoint;

    private void OnEnable()
    {
        Messenger.AddListener<GameObject>(GameEvents.PAINTBALL_PLAYER_SPAWNED, GetCam);
        Messenger.AddListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);

        paintballTM = PaintBallTeamManager.Instance;

        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.PAINTBALL_PLAYER_SPAWNED, GetCam);
        Messenger.RemoveListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        InitializeUI();
    }

    void InitializeUI() //let's make ui empty
    {
        ammoFill.fillAmount = 1;
        redTeamFill.fillAmount = 0;
        redTeamScore.text = "0";
        blueTeamFill.fillAmount = 0;
        blueTeamScore.text = "0";

        playerHPfill.fillAmount = 1;
        playerHPamount.text = "100";   //in case one will have exceeding hp(>100), make field in PaintBallPlayer "MaxHP" and use here through myPlayer
    }

    IEnumerator CDbeforeGameRoutine()
    {
        yield return new WaitForSeconds(2);
        startCD.gameObject.SetActive(true);

        startCD.text = "3";
        yield return new WaitForSeconds(1);
        startCD.text = "2";
        yield return new WaitForSeconds(1);
        startCD.text = "1";
        yield return new WaitForSeconds(1);
        startCD.text = "Go!";
        yield return new WaitForSeconds(1);

        startCD.gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient) CDends(); //if we master and time ends, let's start game for everybody!
    }

    private void CDends()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.START_PAINTBALL_GAME, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void GetCam(GameObject camera)
    {
        playerCamera = camera;
        StartCoroutine(AutoShootEnemyCheck());
    }


    private void SetAmmoFill(float targetValue, float time)
    {
        if (targetValue == 0)
        {
            ammoFill.fillAmount = 0;
        }

        ammoFill.DOFillAmount(targetValue, time);
    }

    private void Update()
    {
        if (reloadButton.Pressed)
        {
            Messenger.Broadcast(GameEvents.RELOAD_PRESSED);
        }

        if (SuperShotButton.Pressed)
        {
            shotPoint = GetHitPoint();
            Messenger.Broadcast(GameEvents.SUPER_SHOT_PRESSED, shotPoint);
        }

        if (powerUpButton.Pressed)
        {
            Messenger.Broadcast(GameEvents.PAINTBALL_POWER_UP_PRESSED);
        }
    }

    IEnumerator AutoShootEnemyCheck()
    {
        if (playerCamera == null) yield return null;

        Ray ray = new Ray();
        while (true)
        {
            ray.origin = playerCamera.transform.position;
            ray.direction = playerCamera.transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, noPlayerLayerMask))
            {
                var hitGO = hit.collider.gameObject;

/*                if(hitGO.CompareTag("Enemy") && hitGO.GetComponent<PlayerHealth>() == null)
                {
                    Messenger.Broadcast(GameEvents.AUTO_SHOOT, hit.point); //just for testing...we can shoot only enemies who has player health script
                }*/
                if (hitGO.CompareTag("Enemy") && !hitGO.GetComponent<PlayerHealth>().isInvulnerable)
                {
                    Messenger.Broadcast(GameEvents.AUTO_SHOOT, hit.point); 
                }
            }
            yield return null;
        }
    }

    private Vector3 GetHitPoint()
    {
        if (CrosshairAnyHitPointCheck() != Vector3.zero)
        {
            return CrosshairAnyHitPointCheck(); //if we find any point - use its hit point as a direction
        }
        else
        {
            Vector3 customHitpoint = playerCamera.transform.position + playerCamera.transform.forward * 30f;  //if no - use camera pos+direction* 30 as hitpoint
            return customHitpoint;
        }

    }

    private Vector3 CrosshairAnyHitPointCheck()
    {
        Ray newRay = new Ray();
        newRay.origin = playerCamera.transform.position;
        newRay.direction = playerCamera.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(newRay, out hit, 100f, noPlayerLayerMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.START_CD_GAME_TIMER)
        {
            //we start timer for everybody as callback
            StartCoroutine(CDbeforeGameRoutine());
        }
        else if(eventCode == GameEvents.START_PAINTBALL_GAME)
        {
            InitializeUI();
        }
        else if(eventCode == GameEvents.HIT_RECIEVED)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNum = (int)data[0];
            int currentHP = (int)data[1];
            int dmgAmount = (int)data[2];
            int fromTeamId = (int)data[3];

            if(PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
            UpdatePlayerHP(currentHP);

            UpdateOverallScore(fromTeamId);
        }
        else if(eventCode == GameEvents.PAINTBALL_GAME_FINISHED)
        {
            object[] data = (object[])photonEvent.CustomData;
            int wonTeamID = (int)data[0];

            PaintBallTeam wonTeam = paintballTM.GetTeamByIndex(wonTeamID);

            resultText.gameObject.SetActive(true);
            resultText.text = wonTeam.teamName.ToString() + " team wins!";
        }

    }

    private void UpdatePlayerHP(int currentHP)
    {
        playerHPamount.text = currentHP.ToString();

        playerHPfill.fillAmount = (float)currentHP / PlayerHealth.staticMaxHP;
    }

    private void UpdateOverallScore(int teamID)
    {
      var currentPoints = paintballTM.GetTeamPoints(teamID);
      var maxPoints = PaintBallGameManager.Instance.pointsToWin;

        switch(teamID)
        {
            case 0: //RED. todo use enum instead of teamIndex here...
                redTeamScore.text = currentPoints.ToString();
                redTeamFill.fillAmount = (float) currentPoints / maxPoints;
                break;

            case 1: //BLUE team
                blueTeamScore.text = currentPoints.ToString();
                blueTeamFill.fillAmount = (float)currentPoints / maxPoints;
                break;
        }

         
    }
}
        
