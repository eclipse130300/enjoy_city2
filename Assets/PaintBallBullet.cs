using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallBullet : MonoBehaviour
{
    [SerializeField] float speed;
    [HideInInspector]
    public Vector3 targetPoint;

    private void OnEnable()
    {
        Invoke("SelfDestroy", 3f);
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
            SelfDestroy();
        }
    }

    void SelfDestroy()
    {
/*        Debug.Log("I DESTROY MYSELF!");*/
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
/*        Debug.Log(other.gameObject.name);*/
        SelfDestroy();
    }
}
