using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 speed;
    [SerializeField] float sinAmpitude = 10f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
        transform.position = transform.position+(Vector3.up *( Mathf.PingPong(Time.time, 1) -0.5f)* sinAmpitude)*Time.deltaTime;
    }
}
