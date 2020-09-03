using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using Utils;

public class ShootAbility : MonoBehaviour , IHaveCooldown, IOnEventCallback
{
    [SerializeField] GameObject dmgBullet;
    [SerializeField] GameObject fakeBullet;

    public GameObject shootingPoint;
    [SerializeField] float targetPointDistance;

    [Header("Ammo")]
    public float shootingDelay = 0.1f;
    [SerializeField] int maxAmmo;
    [SerializeField] float reloadTime;
    [Header("More value is the bigger range is!")]
    [Range(1f, 10f)]
    [SerializeField] float autoShotSprayMultiplier = 1f;

    public int currentAmmo;
    private bool isReloading = false;

    private ThirdPersonInput playerInput;
    private MecanimWrapper mechanim;

    
    [SerializeField] CoolDownSystem coolDownSystem;
    [SerializeField] int iD;

    private float cD;

    public int CoolDownId => iD;

    public float CoolDownDuration => cD;

    private PhotonView photonView;
    private PlayerTeam myTeam;

    //test p
    public Ray ray = new Ray();
    public bool isFiring;

    private void OnEnable()
    {
        playerInput = GetComponent<ThirdPersonInput>();
        photonView = GetComponent<PhotonView>();
        myTeam = GetComponent<PlayerTeam>();
        mechanim = GetComponentInChildren<MecanimWrapper>();

        if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Messenger.AddListener<Vector3>(GameEvents.AUTO_SHOOT, CheckObstacles);
            Messenger.AddListener(GameEvents.RELOAD_PRESSED, Reload);
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Messenger.RemoveListener<Vector3>(GameEvents.AUTO_SHOOT, CheckObstacles);
            Messenger.RemoveListener(GameEvents.RELOAD_PRESSED, Reload);
        }
    }

    private void Start()
    {
        SetMaxAmmo();
        cD = shootingDelay;
    }

    public void Shoot(Vector3 shootDir, float sprayMultiplier = 1f)
    {
        SetBullet(shootDir, sprayMultiplier, dmgBullet);
        DescreaseAmmo(1);
/*
        mechanim.Fire();
*/
        photonView.RPC("GlobalShoot", RpcTarget.Others, shootDir, sprayMultiplier);
    }

    public void SetMaxAmmo()
    {
        currentAmmo = maxAmmo;
    }

    private void DescreaseAmmo(int amount)
    {
        currentAmmo -= amount;
        Messenger.Broadcast(GameEvents.AMMO_UPDATED, GetNormalizedAmmo(maxAmmo, currentAmmo), shootingDelay); //TODO what todo to reuse it without time??

        if(currentAmmo <= 0)
        {
            Reload();
        }
    }

    private float GetNormalizedAmmo(int maxAmmo, int currAmmo)
    {
        return (float)currAmmo / (maxAmmo); 
    }

    private void Reload()
    {
        if (currentAmmo == maxAmmo) return;
        if (isReloading) return;

        //make RPC reload 
        photonView.RPC("TriggerReload", RpcTarget.AllViaServer);
        Messenger.Broadcast(GameEvents.RELOADING, reloadTime);
        StartCoroutine(Reloading(reloadTime));
    }

    [PunRPC]
    private void TriggerReload()
    {
        mechanim.Reload();
    }

    IEnumerator Reloading(float time)
    {
        isReloading = true;

        Messenger.Broadcast(GameEvents.AMMO_UPDATED, 1f , time) ;

        yield return new WaitForSeconds(time);
 
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void SetBullet(Vector3 shootDir, float sprayMultiplier, GameObject bulletTypePref)
    {
        InitializeBullets(myTeam.teamColor, myTeam.myTeamIndex, PhotonNetwork.LocalPlayer.ActorNumber);
        GameObject newBullet = GameObjectPooler.Instance.GetObject(bulletTypePref);

        //it's a main rotation
        shootingPoint.transform.rotation = Quaternion.LookRotation(shootDir, Vector3.up);

        CreateSprayRotation(sprayMultiplier); //it's a spray rotation 

        newBullet.transform.position = shootingPoint.transform.position;
        newBullet.transform.rotation = shootingPoint.transform.rotation;

        var bulletScript = newBullet.GetComponent<PaintBallBullet>();
        bulletScript.targetPoint = shootingPoint.transform.forward * targetPointDistance;
        newBullet.SetActive(true);
    }

    private void CheckObstacles(Vector3 crosshairHitpoint)  //we check obstacles from shooting point to crosshair hit point to prevent obstacle shooting while enemy in sight
    {
        if (coolDownSystem.IsOnCoolDown(iD) || isReloading) return;

        if(crosshairHitpoint == Vector3.zero)
        {
            if (isFiring)
            {
                //end firingAnim
                mechanim.EndFire();
                isFiring = false;
                Messenger.Broadcast(GameEvents.FIRING, crosshairHitpoint);

                return;
            }
        }

        Vector3 shootDir = (crosshairHitpoint - shootingPoint.transform.position).normalized;


        /*Ray ray = new Ray();*/
        ray.origin = shootingPoint.transform.position;
        ray.direction = shootDir;

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f))
        {
            if(hit.collider.gameObject.CompareTag("Enemy"))
            {
                Shoot(shootDir, autoShotSprayMultiplier);
                coolDownSystem.PutOnCooldown(this);


                if (!isFiring)
                {
                    //startFiring anim
                    mechanim.StartFire();
                    isFiring = true;
                    Messenger.Broadcast(GameEvents.FIRING, crosshairHitpoint);
                }
            }
        }
    }

    private void CreateSprayRotation(float sprayMultiplier)
    {
        
        Vector2 dirDiflection = UnityEngine.Random.insideUnitCircle; 
        Vector3 dirDisplacementVector = new Vector3(dirDiflection.x, dirDiflection.y, 0);

        var randomMultiplier = RandomFromDistribution.RandomFromStandardNormalDistribution(); //we use standardDistribution to simulate real-shooting

        var absInputMultyplier = (Mathf.Abs(playerInput.Hinput) + 1) * (Mathf.Abs(playerInput.Vinput) + 1);
        absInputMultyplier = Mathf.Clamp(absInputMultyplier, absInputMultyplier, 2); //clamp input to make spray independent from moving direction

        dirDisplacementVector *= randomMultiplier * absInputMultyplier * sprayMultiplier;  

        shootingPoint.transform.Rotate(dirDisplacementVector.x, dirDisplacementVector.y, 0); //take current rotation and change it based on offset vec
    }

    public void InitializeBullets(Color teamCol, int teamIndex, int damagerActorNum)
    {
        //we initialize our prefabs
        dmgBullet.GetComponent<PaintBallBullet>().InitializeBullet(teamCol, teamIndex, damagerActorNum);
        fakeBullet.GetComponent<PaintBallBullet>().InitializeBullet(teamCol, teamIndex, damagerActorNum);
    }

    [PunRPC]
    public void GlobalShoot(Vector3 shootDir, float sprayMult)
        //globally we shoot fake bullet - others do not need to know about dmg to enemy(if enemny's health is scynchronized)
    {
        SetBullet(shootDir, sprayMult, fakeBullet);
/*        mechanim.Fire();*/
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PLAYER_RESPAWNED)
        {
            currentAmmo = maxAmmo;
        }
    }
}
