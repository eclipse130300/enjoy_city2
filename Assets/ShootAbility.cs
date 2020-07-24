﻿using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class ShootAbility : MonoBehaviour , IHaveCooldown
{
    [SerializeField] GameObject bulletPrefab;
    /*    [SerializeField] GameObject superBulletPrefab;*/
    [SerializeField] GameObject shootingPoint;
    [SerializeField] float targetPointDistance;

    [Header("Ammo")]
    public float shootingDelay = 0.1f;
    [SerializeField] int maxAmmo;
    [SerializeField] float reloadTime;
    [Header("More value is the bigger range is!")]
    [Range(1f, 10f)]
    [SerializeField] float superShotSprayMultiplier = 1f;
    [Range(1f, 10f)]
    [SerializeField] float autoShotSprayMultiplier = 1f;

    private int currentAmmo;
    private bool isReloading = false;

    private ThirdPersonInput playerInput;

    [SerializeField] CoolDownSystem coolDownSystem;
    [SerializeField] int iD;
    private float cD;

    public int CoolDownId => iD;

    public float CoolDownDuration => cD;

    //test p
    public Ray ray = new Ray();

    private void Awake()
    {
        playerInput = GetComponent<ThirdPersonInput>();

        Messenger.AddListener<Vector3>(GameEvents.AUTO_SHOOT, CheckObstacles);
        Messenger.AddListener(GameEvents.RELOAD_PRESSED, Reload);
        Messenger.AddListener<Vector3>(GameEvents.SUPER_SHOT_PRESSED, SuperShot);
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        cD = shootingDelay;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Vector3>(GameEvents.AUTO_SHOOT, CheckObstacles);
        Messenger.RemoveListener(GameEvents.RELOAD_PRESSED, Reload);
        Messenger.RemoveListener<Vector3>(GameEvents.SUPER_SHOT_PRESSED, SuperShot);
    }

/*    private void ShootAbitilyCheck(Vector3 crosshairHitpoint)
    {
        if (coolDownSystem.IsOnCoolDown(iD) || isReloading) return;

        CheckObstacles(crosshairHitpoint);

        coolDownSystem.PutOnCooldown(this);
    }*/

    void Shoot(Vector3 shootDir, float sprayMultiplier = 1f)
    {
        SetBullet(shootDir, sprayMultiplier);
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

    private void SetBullet(Vector3 shootDir, float sprayMultiplier)
    {
        GameObject newBullet = GameObjectPooler.Instance.GetObject(bulletPrefab);

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

        Vector3 shootDir = (crosshairHitpoint - shootingPoint.transform.position).normalized;

        /*Ray ray = new Ray();*/
        ray.origin = shootingPoint.transform.position;
        ray.direction = shootDir;

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.CompareTag("Enemy"))
            {
/*                Debug.Log("I checked obstacles!");*/
                Shoot(shootDir, autoShotSprayMultiplier);
                coolDownSystem.PutOnCooldown(this);
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

    private void SuperShot(Vector3 hitpoint)
    {
        Vector3 shootDir = (hitpoint - shootingPoint.transform.position).normalized;

        while (currentAmmo > 0)
        {
            Shoot(shootDir, superShotSprayMultiplier);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(ray);
    }
}