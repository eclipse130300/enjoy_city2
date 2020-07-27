using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PaintBallBullet : MonoBehaviour, IUpdatable
{
    [SerializeField] float speed;
    [Range(1f,10f)]
    [SerializeField] float collisionTestOffset = 1f;
    [HideInInspector]
    public Vector3 targetPoint;
    [SerializeField] LayerMask noPlayerLayerMask;


    //Test
    public Ray ray;



    /// <summary>
    /// Cached reference so we don't search for it with GetComponent which is slow.
    /// </summary>
    [SerializeField]
    private Transform trans;

    /// <summary>
    /// Cached reference so we don't search for it with GetComponent which is slow.
    /// </summary>
    [SerializeField]
    private Rigidbody body;

    /// <summary>
    /// Imprint on environment.
    /// </summary>
    [SerializeField]
    private Material mark;

    /// <summary>
    /// In world units. Use reasonably small values on mobile devices.
    /// </summary>
    [SerializeField]
    [Range(.01f, float.PositiveInfinity)]
    private float markSize = 1f;

    /// <summary>
    /// Previous position - used to determine hit direction.
    /// </summary>
    private Vector3 posPrev;

    /// <summary>
    /// Ref to the rigidbody.
    /// </summary>
    public Rigidbody Body { get { return body; } }



    public int Index { get; set; }

    public int TypeHash => 0;

    public uint MaxRefFrame => 0;

    public uint SkipFrames => 0;

    /// <summary>
    /// Direction in which the hit will be applied on collision.
    /// (Velocity is no good - collision handler can be sometimes called after it is afected by impact)
    /// </summary>
    private Vector3 hitDirection;

    /// <summary>
    /// Ignore further processing when colliding with more than one object.
    /// </summary>
    private bool alreadyProcessed;


    private void OnEnable()
    {
        Invoke("ImmediateSelfDestroy", 3f);

        alreadyProcessed = false;

        UpdateManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        UpdateManager.Instance.UnRegister(this);
    }

    private void FixedUpdate()
    {
        MoveTo(targetPoint);

        //update hit direction
        hitDirection = trans.position - posPrev;
        posPrev = trans.position;
    }
    private void Update()
    {
        
    }


    private void MoveTo(Vector3 targetPoint)
    {
        if (Vector3.Distance(transform.position, targetPoint) >= 0.1f)
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
        }
        else
        {
            ImmediateSelfDestroy();
        }
    }

    void ImmediateSelfDestroy()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*        Debug.Log("HIT PROCESSED!");

                if (alreadyProcessed)
                    return;

                //notify the controller
                HittablesController.Instance.OnShotHit(new HitData(collision.contacts[0].point, hitDirection, mark, markSize));

                *//*			//remove from scene
                            Destroy(gameObject);*//*
                gameObject.SetActive(false);

                alreadyProcessed = true;*/

        Ray ray = new Ray();

        ray.direction = collision.GetContact(0).point - hitDirection;
        ray.origin = collision.GetContact(0).point + (-ray.direction.normalized * collisionTestOffset);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            ExplodeBullet(hit.point);
        }
    }

    void ExplodeBullet(Vector3 collisionPoint)
    {
        if (alreadyProcessed)
            return;

        //notify the controller
        HittablesController.Instance.OnShotHit(new HitData(collisionPoint, hitDirection, mark, markSize));

        //remove from scene
        gameObject.SetActive(false);

        alreadyProcessed = true;
    }

    public void OnUpdate(float delta)
    {
        ray = new Ray(); 

        ray.direction = transform.forward;
        ray.origin = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, speed * Time.fixedDeltaTime * collisionTestOffset, noPlayerLayerMask))
        {
            Debug.Log("ONUPDATE RAY COLLISION!  " + hit.collider.gameObject.name);
            ExplodeBullet(hit.point);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}
