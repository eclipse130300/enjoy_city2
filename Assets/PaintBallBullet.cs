using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallBullet : MonoBehaviour
{
    [SerializeField] float speed;
    [HideInInspector]
    public Vector3 targetPoint;

    private void Start()
    { 
        Invoke("SelfDestroy", 7f);
    }
    // Update is called once per frame
    void Update()
    {
        MoveTo(targetPoint);
    }

    private void MoveTo(Vector3 targetPoint)
    {
        if (Vector3.Distance(transform.position, targetPoint) >= 3f)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
/*            transform.position += Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * speed);*/
        }
        else
        {
            SelfDestroy();
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        SelfDestroy();
        Debug.Log("I HIT :" + collision.gameObject.name);
    }

    void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
