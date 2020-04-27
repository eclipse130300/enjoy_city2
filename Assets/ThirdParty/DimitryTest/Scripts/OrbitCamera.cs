using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [Range(0.01f, 1.0f)]
    [SerializeField]private float smoothFactor = 0.5f;

    [SerializeField] private float rotationSpeed;
    private float rotationX, rotationY;

    private Vector3 offset;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    
    
    // Start is called before the first frame update
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
        

    }

    private void LateUpdate()
    {
        transform.position = target.position - (rotation * offset);
        /*transform.rotation = rotation;*/
        transform.LookAt(target);
    }
}
