using Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PaintBallBullet : MonoBehaviour, IUpdatable
{
    [SerializeField] float speed;
    [Range(1f,10f)]
    [SerializeField] float collisionOffset = 1f;
    [HideInInspector]
    public Vector3 targetPoint;
    [SerializeField] LayerMask noPlayerLayerMask;

    public Color bulletColor = Color.green;
    [HideInInspector]
    public int fromTeamIndex;

    public string enemyTag = "Enemy";

    [Header("is global bullet(no damage)?")]
    public bool isFake = false;

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
    private Material[] marks;

    /// <summary>
    /// In world units. Use reasonably small values on mobile devices.
    /// </summary>
    [SerializeField]
    [Range(.01f, float.PositiveInfinity)]

    private float markSize = 1f;
/*    [Header("+/- to size to make it random")]
    [SerializeField] float randomSizeRange = 0.2f;*/

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


    private void Awake()
    {
        GetComponent<MeshRenderer>().material.color = bulletColor;
    }

    public void InitializeBullet(Color color, int teamIndex)
    {
        bulletColor = color;
        fromTeamIndex = teamIndex;
    }

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
        Ray ray = new Ray();

        ray.direction = collision.GetContact(0).point - hitDirection;
        ray.origin = collision.GetContact(0).point + (-ray.direction.normalized * collisionOffset);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //if it's not a fake bullet - it damages enemy
            var hitGO = hit.collider.gameObject;
            if(hitGO.GetComponent<PlayerHealth>() && hitGO.CompareTag(enemyTag) && !isFake)
            {
                hitGO.GetComponent<PlayerHealth>().TakeDamage(1); //todo dmg amount only 1?
            }

            //otherwise we just explode it
            ExplodeBullet(hit.point, hit.collider.gameObject.GetComponent<Renderer>());
        }
    }

    void ExplodeBullet(Vector3 collisionPoint, Renderer renderer)
    {
        if (alreadyProcessed)
            return;

        //notify the controller
        HittablesController.Instance.OnShotHit(new HitData(collisionPoint, hitDirection, markSize), bulletColor);

        //remove bullet from scene
        gameObject.SetActive(false);

        alreadyProcessed = true;
    }

    public void OnUpdate(float delta)
    {
        ray = new Ray(); 

        ray.direction = transform.forward;
        ray.origin = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, speed * Time.fixedDeltaTime * collisionOffset, noPlayerLayerMask))
        {
            ExplodeBullet(hit.point, hit.collider.gameObject.GetComponent<Renderer>());
        }
/*
        Ray ray2 = new Ray();

        ray2.direction = -transform.forward;
        ray2.origin = transform.position;

        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2, speed * Time.fixedDeltaTime * collisionTestOffset, noPlayerLayerMask))
        {
            Debug.Log("2 RAY COLLISION!  " + hit2.collider.gameObject.name);
            ExplodeBullet(hit2.point);
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}
