using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPreviewRotator : MonoBehaviour
{
    public FixedTouchField touchField;
    public float rotationSpeed = 10f;

    private void Awake()
    {
        touchField = FindObjectOfType<FixedTouchField>();
    }

    private void Update()
    {
        if(touchField.Pressed)
        {
            transform.Rotate(Vector3.up * -touchField.TouchDist.x * rotationSpeed * Time.deltaTime);
        }
    }
}
