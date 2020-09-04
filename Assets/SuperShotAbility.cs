using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(ShootAbility))]
public class SuperShotAbility : MonoBehaviour, IHaveCooldown
{
    [Range(1f, 10f)]
    [SerializeField] float superShotSprayMultiplier = 1f;

    [SerializeField] float shooperShotReloadDuration = 15f;

    private ShootAbility shootAbility;
    private MecanimWrapper mecanim;
    private PhotonView photon;
    [SerializeField] CoolDownSystem firstCDsys;

    public int iD;

    public int CoolDownId => iD;
    public float CoolDownDuration => shooperShotReloadDuration;

    private void Awake()
    {
        shootAbility = GetComponent<ShootAbility>();
        photon = GetComponent<PhotonView>();
    }

    private void Start()
    {
        mecanim = GetComponentInChildren<MecanimWrapper>();
    }

    private void OnEnable()
    {
        if (photon.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Messenger.AddListener<Vector3>(GameEvents.SUPER_SHOT_PRESSED, SuperShot);
        }
    }

    private void OnDisable()
    {
        if (photon.IsMine && PhotonNetwork.IsConnectedAndReady)
        {
            Messenger.RemoveListener<Vector3>(GameEvents.SUPER_SHOT_PRESSED, SuperShot);
        }
    }

    private void SuperShot(Vector3 hitpoint)
    {
        if (!firstCDsys.IsOnCoolDown(iD) && shootAbility.currentAmmo > 0)
        {
            firstCDsys.PutOnCooldown(this);
            Vector3 shootDir = (hitpoint - shootAbility.shootingPoint.transform.position).normalized;

            while (shootAbility.currentAmmo > 0)
            {
                shootAbility.Shoot(shootDir, superShotSprayMultiplier, true);
            }

            mecanim.SuperShotState();

            Messenger.Broadcast(GameEvents.SUPER_SHOT_CD, shooperShotReloadDuration);
        }
    }
}
