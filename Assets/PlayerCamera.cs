using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{


    [SerializeField] protected Transform target;

    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float moveSpeed = 20f;
    [SerializeField] protected float CameraPosY = 1f;
    [SerializeField] protected float CameraDistance = 2f;
    [SerializeField] protected float CameraPosSpeed = 5f;

    Vector3 movePoint = new Vector3();
    [SerializeField]LayerMask cameraInteractMask;

    float yOfset = 0;

    float maxYOfset = 1;
    float minYOfset = -1;

    public void Awake()
    {
        transform.SetParent(null);
    }
    private void FixedUpdate()
    {
        if (target == null)
            return;
        movePoint = target.position + Vector3.up * CameraPosY * yOfset + target.forward * -1 * CameraDistance;
        RaycastHit hitinfo;
        if (Physics.Raycast(target.position, (movePoint - target.position ).normalized, out hitinfo, CameraDistance, cameraInteractMask))
        {
            movePoint = hitinfo.point + hitinfo.normal * 0.2f;
        }
        Quaternion Rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.position = Vector3.Lerp(transform.position, movePoint, moveSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, rotationSpeed * Time.fixedDeltaTime);
    }
    public void MoveTo(float yOfsetDelta) {
        this.yOfset = Mathf.Clamp(this.yOfset+ yOfsetDelta,minYOfset,maxYOfset);
    }
}
