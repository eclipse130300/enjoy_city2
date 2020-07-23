using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class ShootAbility : MonoBehaviour , IHaveCooldown
{
    [SerializeField] GameObject bulletPrefab;
    /*    [SerializeField] GameObject superBulletPrefab;*/
    [SerializeField] GameObject shootingPoint;
    [SerializeField] float targetPointDistance;

    [Header("When shootPointRotX > 0 we correct rotation(do not shoot in the legs) in degrees!")]
    [SerializeField] float minYAimCorrection = -3f;
    [SerializeField] float maxYAimCorrection = -10f;

    [Header("X shooting point rotation influenced by correction")]
    [SerializeField] float minXinfluencedRotation = 0f;
    [SerializeField] float maxXinfluencedRotation = 18.43f;

    [Header("Ammo")]
    public float shootingDelay = 0.1f;
    [SerializeField] int maxAmmo;
    [SerializeField] float reloadTime;
    [Header("More value is the bigger superShot range is!")]
    [Range(1f, 10f)]
    [SerializeField] float superShotSprayMultiplier = 1f;

    public int currentAmmo; //private
    private bool isReloading = false;

    private ThirdPersonInput playerInput;

    [SerializeField] CoolDownSystem coolDownSystem;
    [SerializeField] int iD;
    private float cD;

    public int CoolDownId => iD;

    public float CoolDownDuration => cD;

    private void Awake()
    {
        playerInput = GetComponent<ThirdPersonInput>();

        Messenger.AddListener<Quaternion>(GameEvents.AUTO_SHOOT, ShootAbitilyCheck);
        Messenger.AddListener(GameEvents.RELOAD_PRESSED, Reload);
        Messenger.AddListener<Quaternion>(GameEvents.SUPER_SHOT_PRESSED, SuperShot);
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        cD = shootingDelay;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Quaternion>(GameEvents.AUTO_SHOOT, ShootAbitilyCheck);
        Messenger.RemoveListener(GameEvents.RELOAD_PRESSED, Reload);
        Messenger.RemoveListener<Quaternion>(GameEvents.SUPER_SHOT_PRESSED, SuperShot);
    }

    private void ShootAbitilyCheck(Quaternion camOrient)
    {
        if (coolDownSystem.IsOnCoolDown(iD) || isReloading) return;

        Shoot(camOrient);

        coolDownSystem.PutOnCooldown(this);
    }

    void Shoot(Quaternion camOrient, float sprayMultiplier = 1f)
    {
        SetBullet(camOrient, sprayMultiplier);
        DescreaseAmmo(1);
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


        Debug.Log("Reloading!");
        StartCoroutine(Reloading(reloadTime));
    }

    IEnumerator Reloading(float time)
    {
        isReloading = true;

        Messenger.Broadcast(GameEvents.AMMO_UPDATED, 1f , time) ;

        yield return new WaitForSeconds(time);
 
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void SetBullet(Quaternion camOrient , float sprayMultiplier)
    {
        GameObject newBullet = GameObjectPooler.Instance.GetObject(bulletPrefab);

        shootingPoint.transform.rotation = camOrient;
        float eulerRotationX = shootingPoint.transform.rotation.eulerAngles.x;
        CorrectBulletDirection(eulerRotationX, sprayMultiplier);

        newBullet.transform.position = shootingPoint.transform.position;
        newBullet.transform.rotation = shootingPoint.transform.rotation;

        var bulletScript = newBullet.GetComponent<PaintBallBullet>();
        bulletScript.targetPoint = shootingPoint.transform.forward * targetPointDistance;
        newBullet.SetActive(true);
    }

    private void CorrectBulletDirection(float currectRotationX, float sprayMultiplier)
    {
        if (shootingPoint.transform.rotation.x > 0)  //we correct aim to prevent floorshooting while looking at the floor with crosshair(looking down)
        {
            float rotationXnormalized = (currectRotationX - minXinfluencedRotation) / (maxXinfluencedRotation - minXinfluencedRotation);
            float finalCorrection = rotationXnormalized * (maxYAimCorrection - minYAimCorrection) + minYAimCorrection;

            shootingPoint.transform.Rotate(-finalCorrection, 0, 0);
        }
        else //if we look up, use min correction
        {
            shootingPoint.transform.Rotate(-minYAimCorrection, 0, 0);
        }
        
        Vector2 dirDiflection = UnityEngine.Random.insideUnitCircle; 
        Vector3 dirOffsetVector = new Vector3(dirDiflection.x, dirDiflection.y, 0);

        var randomMultiplier = RandomFromDistribution.RandomFromStandardNormalDistribution(); //we use standardDistribution to simulate real-shooting

        var absInputMultyplier = (Mathf.Abs(playerInput.Hinput) + 1) * (Mathf.Abs(playerInput.Vinput) + 1);
        absInputMultyplier = Mathf.Clamp(absInputMultyplier, absInputMultyplier, 2); //clamp input to make spray independent from moving direction

        dirOffsetVector *= randomMultiplier * absInputMultyplier * sprayMultiplier;  

        shootingPoint.transform.Rotate(dirOffsetVector.x, dirOffsetVector.y, 0); //take current rotation and change it based on offset vec
    }

    private void SuperShot(Quaternion camOrient)
    {
        while(currentAmmo > 0)
        {
            Shoot(camOrient, superShotSprayMultiplier);
        }
    }


}
