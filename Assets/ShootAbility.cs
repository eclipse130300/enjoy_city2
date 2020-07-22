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
    [SerializeField] float shootingDelay = 0.1f;
    [SerializeField] int maxAmmo;
    [SerializeField] float reloadTime;
    public int currentAmmo; //private

    private bool canShoot = true;

    [SerializeField] CoolDownSystem coolDownSystem;
    [SerializeField] int iD;
    private float cD;

    public int CoolDownId => iD;

    public float CoolDownDuration => cD;

    private void Awake()
    {
/*        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();*/

        Messenger.AddListener<Quaternion>(GameEvents.SHOOT_PRESSED, ShootAbitilyCheck);
        Messenger.AddListener(GameEvents.RELOAD_PRESSED, Reload);
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        cD = shootingDelay;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Quaternion>(GameEvents.SHOOT_PRESSED, ShootAbitilyCheck);
        Messenger.RemoveListener(GameEvents.RELOAD_PRESSED, Reload);
    }

    private void ShootAbitilyCheck(Quaternion camOrient)
    {
        if (coolDownSystem.IsOnCoolDown(iD) || !canShoot) return;

        Shoot(camOrient);
        coolDownSystem.PutOnCooldown(this);
    }

    void Shoot(Quaternion camOrient)
    {
        SetBullet(camOrient);
        DescreaseAmmo(1);
    }

    private void DescreaseAmmo(int amount)
    {
        currentAmmo -= amount;
        Messenger.Broadcast(GameEvents.AMMO_UPDATED, GetNormalizedAmmo(maxAmmo, currentAmmo));

        if(currentAmmo <= 0)
        {
            Reload();
        }
    }

    private float GetNormalizedAmmo(int maxAmmo, int currAmmo)
    {
        return (float)currAmmo / maxAmmo; 
    }

    private void Reload()
    {
        if (currentAmmo == maxAmmo) return;

        Debug.Log("Reloading!");
        StartCoroutine(Reloading(reloadTime));
    }

    IEnumerator Reloading(float time)
    {
        canShoot = false;

        yield return new WaitForSeconds(time);

        currentAmmo = maxAmmo;
        Messenger.Broadcast(GameEvents.AMMO_UPDATED, GetNormalizedAmmo(maxAmmo, currentAmmo));
        canShoot = true;
    }

    private void SetBullet(Quaternion camOrient)
    {
        GameObject newBullet = GameObjectPooler.Instance.GetObject(bulletPrefab);

        shootingPoint.transform.rotation = camOrient;
        float eulerRotationX = shootingPoint.transform.rotation.eulerAngles.x;
        CorrectBulletDirection(eulerRotationX);

        newBullet.transform.position = shootingPoint.transform.position;
        newBullet.transform.rotation = shootingPoint.transform.rotation;

        var bulletScript = newBullet.GetComponent<PaintBallBullet>();
        bulletScript.targetPoint = shootingPoint.transform.forward * targetPointDistance;
        newBullet.SetActive(true);
    }

    private void CorrectBulletDirection(float currectRotationX)
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
        dirOffsetVector *= RandomFromDistribution.RandomFromStandardNormalDistribution() * 2;  //we use standardDistribution to simulate real-shooting



        shootingPoint.transform.Rotate(dirOffsetVector.x, dirOffsetVector.y, 0); //take current rotation and change it based on offset vec
    }


}
