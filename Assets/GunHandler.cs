using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandler : MonoBehaviour
{

    private BoneReferencer boneReferencer;

    public GameObject gunPref;
    public GameObject reservoirPref;

    public GameObject gunPoint;

    private GameObject leftHand;
    private GameObject rightHand;


    private GameObject spawnedGun;
    private GameObject spawnedReserviour;

    private PhotonView phtonView;

    private GameObject leftHandGrab;
    public string leftHandGrabTag = "LeftHandGrab";

/*    private GameObject shootingPoint;*/
/*    public string shootingPointTag = "ShootingPoint"; */

    private MecanimWrapper mecanim;
    private ShootAbility shootAbility;

    [SerializeField] float rotationSpeed;

    private void Awake()
    {
        phtonView = GetComponent<PhotonView>();
        shootAbility = GetComponent<ShootAbility>();
    }

    void Start()
    {
        //lets find right / left hand as our body is yet spawned
        boneReferencer = GetComponentInChildren<BoneReferencer>();
        mecanim = GetComponentInChildren<MecanimWrapper>();

        leftHand = boneReferencer.leftHand;
        rightHand = boneReferencer.rightHand;

        //spawn gun
        SpawnGun();

        SpawnReserviour();

        //subscribe to ui
        if (phtonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Messenger.AddListener<Vector3>(GameEvents.AUTO_SHOOT, RotateGun); //this is for local player - constant rotating
            Messenger.AddListener<Vector3>(GameEvents.FIRING, GlobalRotationRPC); //this is global rotating - just rotate on start fire and end fire
            Messenger.AddListener<float>(GameEvents.RELOADING, DisableAutoRotation);

/*            Messenger.AddListener<Vector3>(GameEvents.SUPER_SHOT_PRESSED, RotateGun);*/
        }

    }

    private void OnDestroy()
    {
        if (phtonView.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Messenger.RemoveListener<Vector3>(GameEvents.AUTO_SHOOT, RotateGun);
            Messenger.RemoveListener<Vector3>(GameEvents.FIRING, GlobalRotationRPC);
            Messenger.RemoveListener<float>(GameEvents.RELOADING, DisableAutoRotation);

/*            Messenger.RemoveListener<Vector3>(GameEvents.SUPER_SHOT_PRESSED, RotateGun);*/
        }
    }

    void GlobalRotationRPC(Vector3 pointToshoot)
    {
        phtonView.RPC("RotateGun", RpcTarget.Others, pointToshoot);
    }

    void DisableAutoRotation(float time)
    {
       StartCoroutine(UnsubscribeFromAutoShootUpdates(time));
    }

    IEnumerator UnsubscribeFromAutoShootUpdates(float time) //TODO don't like it. there are 2!! places where reload handles - here and in shoot ability...todo reload manager???
    {
        Messenger.RemoveListener<Vector3>(GameEvents.AUTO_SHOOT, RotateGun);
        spawnedReserviour.transform.localScale = Vector3.one; //show reserviour!
        mecanim.DisposeLeftHandIKGoal(); //our hand is no more stick to the gun!

        MakeDefaultRotation();
        Debug.Log("I MAKE DEFAULT ROTATION!");

        yield return new WaitForSeconds(time);

        
        mecanim.SetLeftHandIKGoal(leftHandGrab.transform.position);
        spawnedReserviour.transform.localScale = Vector3.zero; //hide reserviour!
        Messenger.AddListener<Vector3>(GameEvents.AUTO_SHOOT, RotateGun);
    }

    [PunRPC]
    void RotateGun(Vector3 shootPoint)
    {
        if(shootPoint == Vector3.zero)
        {
            MakeDefaultRotation();
            return;
        }

        Debug.Log("I ROTATE GUN!!");

        //let's rotate towards the target
        Vector3 lookVec = shootPoint - spawnedGun.transform.position;

        Quaternion rotation = Quaternion.LookRotation(lookVec, Vector3.up);

        StartCoroutine(LeprRoutine(rotation));
    }

    IEnumerator LeprRoutine(Quaternion desiredRot)
    {
        Quaternion currentRot = spawnedGun.transform.rotation;
        while (currentRot != desiredRot)
        {
            spawnedGun.transform.rotation = Quaternion.Slerp(currentRot, desiredRot, rotationSpeed * Time.deltaTime);
             yield return null;
        }
    }

    void SpawnGun()
    {
        spawnedGun = Instantiate(gunPref, rightHand.transform.position, Quaternion.identity, rightHand.transform);

        for (int i = 0; i < spawnedGun.transform.childCount; i++)  //let's find now grab for our hand
        {
            if(spawnedGun.transform.GetChild(i).tag == leftHandGrabTag)
            {
                leftHandGrab = spawnedGun.transform.GetChild(i).gameObject;
            }
            //shooting point from gun is bullshit - it fires before animation plays...

/*            if (spawnedGun.transform.GetChild(i).tag == shootingPointTag)  //and shooting point as well
            {
                shootingPoint = spawnedGun.transform.GetChild(i).gameObject;
            }*/
        }

/*        shootAbility.shootingPoint = this.shootingPoint; //we have default, but it's more precise*/

        MakeDefaultRotation();
    }

    void SpawnReserviour()
    {
        spawnedReserviour = Instantiate(reservoirPref, leftHand.transform.position, Quaternion.identity, leftHand.transform); //we spawn reservoir and hide it till player reloads
        spawnedReserviour.transform.localScale = Vector3.zero;
    }

    void MakeDefaultRotation()
    {
        spawnedGun.transform.forward = rightHand.transform.up;
    }
}
