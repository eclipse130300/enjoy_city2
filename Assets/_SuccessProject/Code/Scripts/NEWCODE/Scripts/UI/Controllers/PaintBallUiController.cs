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
    [SerializeField] Image firstTeamFill;
    [SerializeField] TextMeshProUGUI firstTeamScore;
    [SerializeField] Image secoundTeamFill;
    [SerializeField] TextMeshProUGUI secoundTeamScore;

    [Header("Player")]
    [SerializeField] Image playerHPfill;
    [SerializeField] TextMeshProUGUI playerHPamount;

    [Header("GameTimer")]
    [SerializeField] TextMeshProUGUI gameTimer;


    public TextMeshProUGUI startCD;
    public GameObject playerCamera;
    public LayerMask noPlayerLayerMask;

    //test
    Vector3 shotPoint;

    private void OnEnable()
    {
        Messenger.AddListener<GameObject>(GameEvents.PAINTBALL_PLAYER_SPAWNED, GetCam);
        Messenger.AddListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);

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
        firstTeamFill.fillAmount = 0;
        firstTeamScore.text = "0";
        secoundTeamFill.fillAmount = 0;
        secoundTeamScore.text = "0";

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
                if (hit.collider.gameObject.CompareTag("Enemy"))
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
            Debug.Log("start game CD event recieved!");
            StartCoroutine(CDbeforeGameRoutine());
        }
        if(eventCode == GameEvents.START_PAINTBALL_GAME)
        {
            Debug.Log("UI initialize event recieved!");
            InitializeUI();
        }

    }
}
        
