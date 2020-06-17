﻿
using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(PhotonView))]
public class ThirdPersonInput :MonoBehaviour, IPunObservable
{

    public PhotonView photonView;

/*    public FixedJoystick LeftJoystick;*/
    public FloatingJoystick LeftJoystick;

    public FixedButton JumpButton;
    public FixedButton CrouchButton;
    public FixedTouchField TouchField;
    public float JumpForce = 5f;
    public float jumpHeight;
    public float rotateSpeed = 5f;
    public float cameraYSpeed = 5f;
    protected ThirdPersonUserControl Control;

    [SerializeField] SkinsManager skinsManager;

    protected bool isCrouching = false;
    protected CapsuleCollider CapCollider;

    private bool hasReferencies = false;
    private float targetJump;

    [SerializeField] PlayerCamera camer;
    [SerializeField] private Transform m_Cam;
    private CharacterController _characterController; // A reference to the ThirdPersonCharacter on the object
                            // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;                      // The current forward direction of the camera
    private Vector3 m_Move;
    [HideInInspector]
    public bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    [HideInInspector]
    public float Hinput;
    [HideInInspector]
    public Vector3 cameraRotation;
    [HideInInspector]
    public float Vinput;
    public float speed;
    [SerializeField] Transform hips;
    [SerializeField] float rotationSpeed;

    Vector3 inertionMovement;
    // Use this for initialization
    [SerializeField] MecanimWrapper mecanim;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine || !PhotonNetwork.IsConnectedAndReady)
        {
            Loader.Instance.AllSceneLoaded += SubscibeToUI;
        }
        _characterController = GetComponent<CharacterController>();

       // ObservedComponents.Add(this);
    }
    private void OnDestroy()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnectedAndReady)
        {
            Loader.Instance.AllSceneLoaded -= SubscibeToUI;
        }
    }
    private void Start()
    {

        if (!photonView.IsMine && PhotonNetwork.IsConnectedAndReady) {
            camer.gameObject.SetActive(false);
            m_Cam.gameObject.SetActive(false);
        }
        // get the third person character ( this should never be null due to require component )

    }
    Vector3 pos;
    Quaternion rotation;
    Vector3 newCameraRotation;
    float nHinput;
    float nVinput;
    private void FixedUpdate()
    {
        if (LeftJoystick != null && (photonView.IsMine || !PhotonNetwork.IsConnectedAndReady))
        {


            Hinput = Mathf.Clamp(LeftJoystick.input.x + Input.GetAxis("Horizontal"), -1, 1);
            Vinput = Mathf.Clamp(LeftJoystick.input.y + Input.GetAxis("Vertical"), -1, 1);
            m_Jump = (Input.GetKeyDown(KeyCode.Space) || JumpButton.Pressed);

            camer.MoveTo(TouchField.TouchDist.y * Time.fixedDeltaTime * -1);

            cameraRotation = new Vector3((transform.forward * 5 + transform.position).x, (transform.forward * 5 + transform.position).y + (camer.transform.forward * 5).y, (transform.forward * 5 + transform.position).z) + Vector3.up;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * TouchField.TouchDist.x * rotateSpeed * Time.fixedDeltaTime);

        }
        else {
            if(!photonView.IsMine)
            {
                Hinput = nHinput;
                Vinput = nVinput;

                if(cameraRotation == Vector3.zero)
                    cameraRotation = newCameraRotation;

                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * 5);
                if (Vector3.Distance(transform.position, pos) > 0.2f)
                {
                    transform.position = Vector3.Lerp(transform.position, pos, Time.fixedDeltaTime*5);
                    
                    cameraRotation = Vector3.Lerp(cameraRotation, newCameraRotation, Time.fixedDeltaTime*5);
                }
                else {
                   
                    cameraRotation = Vector3.Lerp(cameraRotation, newCameraRotation, Time.fixedDeltaTime*3);
                }                
            }
           
        }
        if (_characterController.isGrounded)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = Vinput * transform.forward + Hinput * transform.right;
            mecanim.SetHorizontalSpeed(Hinput);
            mecanim.SetVerticalSpeed(Vinput);
        }

        if (m_Move != Vector3.zero)
        {

            m_Move -= m_Move*Time.fixedDeltaTime;
        }

        
        
        Jump();


        mecanim.SetJump(targetJump > 0 || !_characterController.isGrounded);
        // pass all parameters to the character control script
        _characterController.Move(m_Move * Time.fixedDeltaTime * speed);
        m_Jump = false;

       
    }
    
    private void LateUpdate()
    {
       

      //  if (angle > 1 || angle < -1)
        //    return;

        Vector3 rotation = Quaternion.LookRotation(cameraRotation - hips.transform.position, Vector3.up).eulerAngles;


        float angle = Vector3.Angle(transform.forward, (cameraRotation - hips.transform.position).normalized);

        if (angle > 30 || angle < -30)
            return;
        for (int i = 0; i < skinsManager.skinHolder.childCount; i++)
        {
            Transform[] newTransform = skinsManager.skinHolder.GetChild(i).GetComponentsInChildren<Transform>();
            foreach (var child in newTransform) { 
                if(hips.name == child.name)
                {
                    child.rotation = Quaternion.Euler(rotation);
                    break;
                }              
            }
        }
        hips.rotation = Quaternion.Euler(rotation);
    }

    void SubscibeToUI()
    {
        Debug.Log("IsMine " + photonView.IsMine);
        if ((photonView.IsMine || !PhotonNetwork.IsConnectedAndReady))
        {
            /*            LeftJoystick = FindObjectOfType<FixedJoystick>();*/
            LeftJoystick = FindObjectOfType<FloatingJoystick>();
            TouchField = FindObjectOfType<FixedTouchField>();
            JumpButton = FindObjectOfType<FixedButton>();

            hasReferencies = true;
        }
    }

    private void Walk()
    {

      //  var input = new Vector3(Hinput, 0, Vinput);
      //  CameraAngleY += TouchField.TouchDist.x * CameraAngleSpeed * 2;
     //   var vel = Quaternion.AngleAxis(CameraAngleY + 180, Vector3.up) * input * 5f;
                

     //   if (input != Vector3.zero)
        {
          //  transform.rotation = Quaternion.AngleAxis(CameraAngleY + Vector3.SignedAngle(Vector3.forward, input.normalized + Vector3.forward * 0.0001f, Vector3.up) + 180, Vector3.up);
        }

       


    }

    private void Jump()
    {
        {
            m_Move.y = targetJump;

            targetJump -= Time.fixedDeltaTime * JumpForce;

            if (_characterController.isGrounded && m_Jump)
            {
                targetJump = jumpHeight;
                inertionMovement = new Vector3(m_Move.x, targetJump, m_Move.z);

            }

            if (!_characterController.isGrounded)
            {
                _characterController.Move(inertionMovement * Time.fixedDeltaTime);
            }
        }
    }

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    
        if (stream.IsWriting)
        {
           // Debug.Log("OnPhotonSerializeView IsWriting");
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.localRotation.eulerAngles);
            stream.SendNext(cameraRotation);
            stream.SendNext(Hinput);
            stream.SendNext(Vinput);
            stream.SendNext(m_Jump); 

        }
        else
        {
            pos = (Vector3)stream.ReceiveNext();
            rotation = Quaternion.Euler((Vector3)stream.ReceiveNext());
            newCameraRotation = (Vector3)stream.ReceiveNext();
            nHinput = (float)stream.ReceiveNext();
            nVinput = (float)stream.ReceiveNext();
            m_Jump = (bool)stream.ReceiveNext();
        }
    }
}
