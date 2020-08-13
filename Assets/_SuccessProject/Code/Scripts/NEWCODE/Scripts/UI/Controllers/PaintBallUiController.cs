using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBallUiController : MonoBehaviour
{
/*    [SerializeField] FixedButton shootButton;*/
    [SerializeField] FixedButton reloadButton;
    [SerializeField] FixedButton SuperShotButton;
    [SerializeField] FixedButton powerUpButton;

    [SerializeField] Image ammoFill;
    [SerializeField] float lerpSpeed;
    [SerializeField] RectTransform crosshairRect;

    Camera playerCamera;
    public LayerMask noPlayerLayerMask;

    //test
    Vector3 shotPoint;

    private void Awake()
    {


        Messenger.AddListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill); 
    }

    private void Start()
    {
        ammoFill.fillAmount = 1;

        StartCoroutine(TryFindPlayerCam());
        StartCoroutine(AutoShootEnemyCheck());
    }

    IEnumerator TryFindPlayerCam()
    {
        while(playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            yield return null;
        }
    }

    private void SetAmmoFill(float targetValue, float time)
    {
        if(targetValue == 0)
        {
            ammoFill.fillAmount = 0;
        }

        ammoFill.DOFillAmount(targetValue, time);
    }

    private void Update()
    {
        if(reloadButton.Pressed)
        {
            Messenger.Broadcast(GameEvents.RELOAD_PRESSED);
        }

        if(SuperShotButton.Pressed)
        {
            Messenger.Broadcast(GameEvents.SUPER_SHOT_PRESSED, GetHitPoint());

            shotPoint = GetHitPoint();
        }

        if(powerUpButton.Pressed)
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
        if(CrosshairAnyHitPointCheck() != Vector3.zero)
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

/*    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(shotPoint, 1f);
    }*/
}
