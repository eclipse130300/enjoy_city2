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
    #region Fields

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

    [SerializeField] Image superShotfill;
    [SerializeField] Image powerUpfill;

    [Header("GameTimer")]
    [SerializeField] TextMeshProUGUI gameTimer;

    [Header("Result text")]
    [SerializeField] TextMeshProUGUI resultText;


    public int GameOverallDuration
    {
        get { return minutes * 60 + secounds; }
    }

    [Header("Game time")]
    [SerializeField] int minutes;
    [SerializeField] int secounds;
    private float timeToEndGame = 0f;
    private float timeElapsed;

    public TextMeshProUGUI startCD;
    public GameObject playerCamera;
    public LayerMask noPlayerLayerMask;

    private PaintBallTeamManager paintballTM;

    Vector3 shotPoint;
    private bool gameIsActive;

    #endregion

    #region Unity_events

    private void OnEnable()
    {
        Messenger.AddListener<GameObject>(GameEvents.PAINTBALL_PLAYER_SPAWNED, GetCam);
        Messenger.AddListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);
        Messenger.AddListener<float>(GameEvents.SUPER_SHOT_CD, SuperShotReload);
        Messenger.AddListener<float>(GameEvents.PAINTBALL_POWER_UP_CD, PowerUPReload);

        paintballTM = PaintBallTeamManager.Instance;

        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<GameObject>(GameEvents.PAINTBALL_PLAYER_SPAWNED, GetCam);
        Messenger.RemoveListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);
        Messenger.RemoveListener<float>(GameEvents.SUPER_SHOT_CD, SuperShotReload);
        Messenger.RemoveListener<float>(GameEvents.PAINTBALL_POWER_UP_CD, PowerUPReload);

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {
        InitializeUI();
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
        //game timer tick
        if(timeToEndGame > 0 && gameIsActive)
        {
            var remainingTime =  Mathf.Max(timeToEndGame - timeElapsed, 0);
            timeElapsed += Time.deltaTime;
            int minutes = Mathf.RoundToInt(remainingTime) / 60;
            var secounds = Mathf.RoundToInt(remainingTime - minutes*60);
            gameTimer.text = string.Format("{0:00}:{1:00}", minutes, secounds);
            if(remainingTime <= 0)
            {
                GameFinishEvent();
                gameIsActive = false;
            }
        }
    }

    #endregion

    #region Photon

    void GameFinishEvent()
    {
        Debug.Log("Game finished by time!");

        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameEvents.PAINTBALL_GAME_FINISHED, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.START_CD_GAME_TIMER)
        {
            //we start timer for everybody as callback
            StartCoroutine(CDbeforeGameRoutine());
        }
        else if (eventCode == GameEvents.START_PAINTBALL_GAME)
        {
            InitializeUI();
            gameIsActive = true;
            StartGameTimer();
        }
        else if (eventCode == GameEvents.HIT_RECIEVED)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNum = (int)data[0];
            int currentHP = (int)data[1];
/*            int dmgAmount = (int)data[2];*/
            int fromTeamId = (int)data[3];

            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
            {
                UpdatePlayerHP(currentHP);
            }

            UpdateOverallScore(fromTeamId);
        }
        else if (eventCode == GameEvents.PAINTBALL_GAME_FINISHED)
        {
            object[] data = (object[])photonEvent.CustomData;
            PaintBallTeam winnerTEam = null;

            //TODO - DRAW!?!
            if (data.IsNullOrEmpty()) //meaning the time is up(no team hit the final shot)!we need to know whitch team has won...
            {
                winnerTEam = PaintBallGameManager.Instance.WhichTeamHasWon();
            }
            else //we know the team who first earned necessary pts
            {
                int wonTeamID = (int)data[0];
                winnerTEam = paintballTM.GetTeamByIndex(wonTeamID);
            }
            resultText.gameObject.SetActive(true);
            resultText.text = winnerTEam.teamName.ToString() + " team wins!";

            gameIsActive = false;

            Destroy(this); //let's destroy this, we no longer need it
        }
        else if(eventCode == GameEvents.PLAYER_RESPAWNED) //we use heal ui only to our player
        {
            object[] data = (object[])photonEvent.CustomData;
            int currentHP = (int)data[0];
            //full heal player
            UpdatePlayerHP(currentHP);
        }

    }

    #endregion

    #region Custom_funcs

    void InitializeUI() //let's make ui empty
    {
        ammoFill.fillAmount = 1;
        redTeamFill.fillAmount = 0;
        redTeamScore.text = "0";
        blueTeamFill.fillAmount = 0;
        blueTeamScore.text = "0";

        playerHPfill.fillAmount = 1;
        playerHPamount.text = PlayerHealth.staticMaxHP.ToString();   //refactor in case one will have starting exceeding hp(>100)
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

    private void StartGameTimer()
    {
        timeToEndGame = GameOverallDuration;
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

    private void SuperShotReload(float time)
    {
        superShotfill.fillAmount = 0;

        superShotfill.DOFillAmount(1, time);
    }

    private void PowerUPReload(float time)
    {
        powerUpfill.fillAmount = 0;
        powerUpfill.DOFillAmount(1, time);
    }

    IEnumerator AutoShootEnemyCheck()
    {
        if (playerCamera == null) yield return null; //we try to find camera before game

        Ray ray = new Ray();
        while (true)
        {
            if (playerCamera == null) Destroy(this); //if camera has been destroyed just before this on sceneload, we should'nt check anymore 

            ray.origin = playerCamera.transform.position;
            ray.direction = playerCamera.transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, noPlayerLayerMask))
            {
                var hitGO = hit.collider.gameObject;

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

    private void UpdatePlayerHP(int currentHP)
    {
        playerHPamount.text = currentHP.ToString();
        playerHPfill.fillAmount = (float)currentHP / PlayerHealth.staticMaxHP;
    }

    private void UpdateOverallScore(int teamID)
    {
      var currentPoints = paintballTM.GetTeamPoints(teamID) /* + 1*/; //+1 here because game manager hasn't updated score yet...toDO script execution?
      var maxPoints = PaintBallGameManager.Instance.pointsToWin;

/*        Debug.Log("I update UI. current team points - " + currentPoints);*/

        switch (teamID)
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
    #endregion
}



