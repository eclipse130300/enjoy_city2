using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 5f;
    private Vector3 dollyDirection;
    private float distance;

    public Vector3 hitpoint;

    
    [Range(0.01f, 1.0f)]
    [SerializeField]private float smoothFactor = 0.5f;

    [SerializeField] private float rotationSpeed;
    private float rotationX, rotationY;

    private Vector3 offset;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    
    
    private void Awake()
    {
        dollyDirection = (target.position - transform.position).normalized;
        distance = (target.position - transform.position).magnitude;
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        offset = target.position - transform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        rotationX = Mathf.Clamp(rotationX, -15, 60); //make fields
        
        
        
        desiredRotation = Quaternion.Euler(rotationX,rotationY,0);
        rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothFactor);
        
        
        dollyDirection = (target.position - transform.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(target.position, transform.position, out hit))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
            hitpoint = hit.point;
            Debug.Log(hit.distance);
        }
        else
        {
            distance = maxDistance;
        }
        
        
        offset = Vector3.Lerp(transform.position, dollyDirection * distance, smoothFactor * Time.deltaTime);
        
        
    }
    
    private void LateUpdate()
    {
        transform.position = target.position - (rotation * offset);
        transform.rotation = rotation;
        transform.LookAt(target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(target.position, hitpoint);
    }
}
