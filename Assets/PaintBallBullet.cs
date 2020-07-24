using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallBullet : MonoBehaviour
{
    [SerializeField] float speed;
    [HideInInspector]
    public Vector3 targetPoint;

    [SerializeField] GameObject bulletStainProjectorPrefab;

    private void OnEnable()
    {
        Invoke("ImmediateSelfDestroy", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTo(targetPoint);
    }

    private void MoveTo(Vector3 targetPoint)
    {
        if (Vector3.Distance(transform.position, targetPoint) >= 0.1f)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
/*            transform.position += Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * speed);*/
        }
        else
        {
            ImmediateSelfDestroy();
        }
    }

    void CollisionDestroy(ContactPoint contactPoint)
    {
        //play VFX

        //leave stain projection
        LeaveStainProjector(contactPoint);



        gameObject.SetActive(false);
    }

    void ImmediateSelfDestroy()
    {
        gameObject.SetActive(false);
    }


    private void LeaveStainProjector(ContactPoint contactpoint)
    {
        GameObject stainProjector = GameObjectPooler.Instance.GetObject(bulletStainProjectorPrefab);
        stainProjector.transform.position = contactpoint.point;
        stainProjector.transform.forward = contactpoint.normal;

        stainProjector.SetActive(true);
    }

/*    private void OnTriggerEnter(Collider other)
    {
*//*        Debug.Log(other.gameObject.name);*//*
        
        other.Get
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        CollisionDestroy(collision.GetContact(0));
    }
}
