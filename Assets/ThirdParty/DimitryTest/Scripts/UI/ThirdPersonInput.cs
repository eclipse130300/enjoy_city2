
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class ThirdPersonInput : MonoBehaviour
{
    public FixedJoystick LeftJoystick;

    public FixedButton JumpButton;
    public FixedButton CrouchButton;
    public FixedTouchField TouchField;
    public float JumpForce = 5f;
    public float jumpHeight;

    protected ThirdPersonUserControl Control;

    protected float CameraAngleY;
    public float CameraAngleSpeed = 0.1f;
    protected float CameraPosY = 3f;
    protected float CameraPosSpeed = 0.02f;

    protected bool isCrouching = false;
    protected CapsuleCollider CapCollider;

    private bool hasReferencies = false;
    private float targetJump;



    private CharacterController _characterController; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                             // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;                      // The current forward direction of the camera
    private Vector3 m_Move;
    [HideInInspector]
    public bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    [HideInInspector]
    public float Hinput;
    [HideInInspector]
    public float Vinput;
    public float speed;
    [SerializeField] float rotationSpeed;

    Vector3 inertionMovement;
    // Use this for initialization

    private void Awake()
    {
        Loader.Instance.AllSceneLoaded += SubscibeToUI;
        
        _characterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )

    }

    private void FixedUpdate()
    {
        if (hasReferencies)
        {
            Hinput = LeftJoystick.input.x;
            Vinput = LeftJoystick.input.y;
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = Vinput * m_CamForward + Hinput * m_Cam.right;
            }

            if (m_Move != Vector3.zero)
            {
                Quaternion direction = Quaternion.LookRotation(m_Move);
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotationSpeed);
            }


            Walk();
            Jump();
            // pass all parameters to the character control script
            _characterController.Move(m_Move * Time.fixedDeltaTime * speed);
            m_Jump = false;

        } 
    }


void SubscibeToUI()
    {
        if (FindObjectOfType<GameUIController>() != null)
        {
            LeftJoystick = FindObjectOfType<FixedJoystick>();
            TouchField = FindObjectOfType<FixedTouchField>();
            JumpButton = FindObjectOfType<FixedButton>();

            hasReferencies = true;
        }
    }

    private void Walk()
    {

        var input = new Vector3(Hinput, 0, Vinput);
        CameraAngleY += TouchField.TouchDist.x * CameraAngleSpeed * 2;
        var vel = Quaternion.AngleAxis(CameraAngleY + 180, Vector3.up) * input * 5f;


        if (input != Vector3.zero)
        {
            transform.rotation = Quaternion.AngleAxis(CameraAngleY + Vector3.SignedAngle(Vector3.forward, input.normalized + Vector3.forward * 0.0001f, Vector3.up) + 180, Vector3.up);
        }

        CameraPosY = Mathf.Clamp(CameraPosY - TouchField.TouchDist.y * CameraPosSpeed, 0, 5f);

        Camera.main.transform.position = transform.position + Quaternion.AngleAxis(CameraAngleY, Vector3.up) * new Vector3(0, CameraPosY, 4);
        Quaternion Rotation = Quaternion.LookRotation(transform.position + Vector3.up * 2f - Camera.main.transform.position, Vector3.up);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Rotation, rotationSpeed);

    }

    private void Jump()
    {
        m_Move.y = targetJump;

        targetJump -= Time.fixedDeltaTime * 5;

        if (_characterController.isGrounded && (Input.GetKeyDown(KeyCode.Space) || JumpButton.Pressed))
        {
            targetJump = jumpHeight;
            inertionMovement = new Vector3(m_Move.x, targetJump, m_Move.z);

        }

        if (!_characterController.isGrounded)
        {
            _characterController.Move(inertionMovement * Time.fixedDeltaTime);
        }

    }

    //private void Crouch()
    //{
    //    //var crouchbutton = CrouchButton.Pressed || Input.GetKey(KeyCode.C);

    //    if (!isCrouching && crouchbutton)
    //    {
    //        //crouch
    //        CapCollider.height = 0.5f;
    //        CapCollider.center = new Vector3(CapCollider.center.x, 0.25f, CapCollider.center.z);
    //        isCrouching = true;
    //    }

    //    Debug.DrawRay(transform.position, Vector3.up * 2f, Color.green);
    //    if (isCrouching && !crouchbutton)
    //    {
    //        //try to stand up
    //        var cantStandUp = Physics.Raycast(transform.position, Vector3.up, 2f);

    //        if (!cantStandUp)
    //        {
    //            CapCollider.height = 1f;
    //            CapCollider.center = new Vector3(CapCollider.center.x, 0.5f, CapCollider.center.z);
    //            isCrouching = false;
    //        }
    //    }
    //}
}
