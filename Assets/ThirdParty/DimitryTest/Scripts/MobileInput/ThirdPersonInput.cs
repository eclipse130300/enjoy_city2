
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class ThirdPersonInput : MonoBehaviour
{

    public FixedJoystick LeftJoystick;
    //public FixedButton Button;
    public FixedTouchField TouchField;
    public ThirdPersonUserControl Control; 

    protected float CameraAngle;
    protected float CameraAngleSpeed = 0.2f;

    private bool hasReferances = false;
    // Use this for initialization

    private void Awake()
    {
        Control = GetComponent<ThirdPersonUserControl>();
        Loader.Instance.AllSceneLoaded += SubscibeToUI;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasReferances)
        {
            Control.Hinput = LeftJoystick.input.x;
            Control.Vinput = LeftJoystick.input.y;

            CameraAngle += TouchField.TouchDist.x * CameraAngleSpeed;

            Camera.main.transform.position = transform.position + Quaternion.AngleAxis(CameraAngle, Vector3.up) * new Vector3(0, 3, 4);
            Camera.main.transform.rotation = Quaternion.LookRotation(transform.position + Vector3.up * 2f - Camera.main.transform.position, Vector3.up);
        }
    }

    void SubscibeToUI()
    {
        LeftJoystick = FindObjectOfType<FixedJoystick>();
        TouchField = FindObjectOfType<FixedTouchField>();

        hasReferances = true;
    }
}
