using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float smooth;
    private Vector3 dollyDirection;
    private float distance;

    public Vector3 hitpoint;
    private void Awake()
    {
        dollyDirection = (target.position - transform.position).normalized;
        distance = (target.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        dollyDirection = (target.position - transform.position).normalized;
        
        RaycastHit hit;

        if (Physics.Linecast(target.position, transform.position, out hit))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
            hitpoint = hit.point;
            Debug.Log(hit.distance);
        }
        else
        {
            distance = maxDistance;
        }
        
        transform.position = Vector3.Lerp(transform.position, dollyDirection * distance, smooth * Time.deltaTime);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(target.position, hitpoint);
    }
}
