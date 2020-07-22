using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBallUiController : MonoBehaviour
{
    [SerializeField] FixedButton shootButton;
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

        Messenger.AddListener<float>(GameEvents.AMMO_UPDATED, SetAmmoFill);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<float>(GameEvents.AMMO_UPDATED, SetAmmoFill); 
    }

    private void Start()
    {
        ammoFill.fillAmount = 1;

        StartCoroutine(AutoShootTargetCheck());
    }

    private void SetAmmoFill(float value)
    {
        StartCoroutine(LerpAmmoFill(value));
    }

    IEnumerator LerpAmmoFill(float targetValue)
    {
        while (ammoFill.fillAmount != targetValue)
        {
            ammoFill.fillAmount = Mathf.MoveTowards(ammoFill.fillAmount, targetValue, lerpSpeed);
            yield return null;
        }

        ammoFill.fillAmount = targetValue;
/*        Debug.Log("finished!");*/
    }

    private void Update()
    {
        if(shootButton.Pressed)
        {
            Quaternion cameraOrientation = GetCameraOrientation();
            Messenger.Broadcast(GameEvents.SHOOT_PRESSED, cameraOrientation);
        }

        if(reloadButton.Clicked)
        {
            Messenger.Broadcast(GameEvents.RELOAD_PRESSED);
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
                Debug.Log(hit.collider.tag) ;
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Messenger.Broadcast(GameEvents.SHOOT_PRESSED, GetCameraOrientation());
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
