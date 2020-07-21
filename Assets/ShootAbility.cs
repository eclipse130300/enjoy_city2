using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAbility : MonoBehaviour , IHaveCooldown
{
    [SerializeField] GameObject bulletPrefab;
    /*    [SerializeField] GameObject superBulletPrefab;*/
    [SerializeField] GameObject shootingPoint;
    [SerializeField] float targetPointDistance;
    [SerializeField] float yAimCorrection;

    [SerializeField] CoolDownSystem coolDownSystem;
    [SerializeField] int iD;
    [SerializeField] float cD;

    public int CoolDownId => iD;

    public float CoolDownDuration => cD;

    private void Awake()
    {
        Messenger.AddListener<Quaternion>(GameEvents.SHOOT_PRESSED, ShootAbitilyCheck);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<Quaternion>(GameEvents.SHOOT_PRESSED, ShootAbitilyCheck);
    }

    private void ShootAbitilyCheck(Quaternion camOrient)
    {
        if (coolDownSystem.IsOnCoolDown(iD)) return;

        Shoot(camOrient);
    }

    void Shoot(Quaternion camOrient)
    {
        GameObject newBullet = GameObjectPooler.Instance.GetObject(bulletPrefab);
        shootingPoint.transform.rotation = camOrient;
        /*shootingPoint.transform.rotation = Quaternion.AngleAxis(yAimCorrection, shootingPoint.transform.right); *///correctDirection

        /*        Vector3 correctedRotation = new Vector3(shootingPoint.transform.rotation.x,
                    shootingPoint.transform.rotation.y + yAimCorrection, shootingPoint.transform.rotation.z);

                shootingPoint.transform.rotation = Quaternion.Euler(correctedRotation);*/

        shootingPoint.transform.Rotate(yAimCorrection, 0, 0);


        newBullet.transform.position = shootingPoint.transform.position;
        newBullet.transform.rotation = shootingPoint.transform.rotation;

        newBullet.GetComponent<PaintBallBullet>().targetPoint = shootingPoint.transform.forward * targetPointDistance;
/*        newBullet.GetComponent<PaintBallBullet>().targetPoint.y = newBullet.GetComponent<PaintBallBullet>().targetPoint.y + 50f;*/



        newBullet.SetActive(true);
        coolDownSystem.PutOnCooldown(this);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((shootingPoint.transform.forward * targetPointDistance), 2f);
    }
}
