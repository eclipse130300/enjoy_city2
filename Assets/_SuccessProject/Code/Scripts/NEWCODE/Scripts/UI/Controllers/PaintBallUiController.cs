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
    public LayerMask rayLayerMask;

    //test
    Ray ray;

    private void Awake()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Messenger.AddListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<float, float>(GameEvents.AMMO_UPDATED, SetAmmoFill); 
    }

    private void Start()
    {
        ammoFill.fillAmount = 1;

        StartCoroutine(AutoShootTargetCheck());
    }

    private void SetAmmoFill(float targetValue, float time)
    {
        if(targetValue == 0)
        {
            ammoFill.fillAmount = 0;
        }

        ammoFill.DOFillAmount(targetValue, time);

/*        StartCoroutine(LerpAmmoFill(targetValue, time));*/
    }

/*    IEnumerator LerpAmmoFill(float targetValue, float time)
    {
        var distance = Mathf.Max(targetValue, ammoFill.fillAmount) - Mathf.Min(targetValue, ammoFill.fillAmount);

        Debug.Log("distance" + distance);

        while (ammoFill.fillAmount != targetValue)
        {
            ammoFill.fillAmount = Mathf.MoveTowards(ammoFill.fillAmount, targetValue, (distance/time) * Time.deltaTime);
            yield return null;
        }

        Debug.Log("finished!");
        ammoFill.fillAmount = targetValue;


        
    }*/

    private void Update()
    {
        if(reloadButton.Pressed)
        {
            Messenger.Broadcast(GameEvents.RELOAD_PRESSED);
        }

        if(SuperShotButton.Pressed)
        {
            Quaternion cameraOrientation = GetCameraOrientation();
            Messenger.Broadcast(GameEvents.SUPER_SHOT_PRESSED, cameraOrientation);
        }

        if(powerUpButton.Pressed)
        {
            Messenger.Broadcast(GameEvents.PAINTBALL_POWER_UP_PRESSED);
        }
    }

    IEnumerator AutoShootTargetCheck()
    {
        while (true)
        {
            /*ray = playerCamera.ScreenPointToRay(new Vector3(0,0,0));*/
            ray.origin = playerCamera.transform.position;
            ray.direction = playerCamera.transform.forward;


            /*ray.direction = playerCamera.transform.TransformDirection(playerCamera.transform.forward);*/

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, rayLayerMask))
            {
/*                Debug.Log(hit.collider.tag) ;*/
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Messenger.Broadcast(GameEvents.AUTO_SHOOT, GetCameraOrientation());
                }
            }
            yield return null;
        }
    }

    private Quaternion GetCameraOrientation()
    {
        return playerCamera.transform.rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}
