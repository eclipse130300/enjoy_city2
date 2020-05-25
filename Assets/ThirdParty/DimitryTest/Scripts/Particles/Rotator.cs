using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 speed;
    [SerializeField] float sinAmpitude = 0.007f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed);
        transform.Translate(Vector3.forward * Mathf.Sin(Mathf.PI * Time.time) * sinAmpitude);
    }
}
