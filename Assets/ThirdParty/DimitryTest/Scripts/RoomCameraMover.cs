using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCameraMover : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed;

    private void Awake()
    {
        target = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        if (target == null) return;

        Quaternion rotation = transform.rotation;
        Quaternion newRotation = Quaternion.LookRotation(target.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(rotation, newRotation, Time.deltaTime * rotationSpeed);
        if (transform.rotation == newRotation) target = null;
    }
}
