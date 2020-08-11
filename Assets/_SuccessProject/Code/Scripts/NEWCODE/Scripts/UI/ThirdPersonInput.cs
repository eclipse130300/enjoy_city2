
using Demo;
using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
/*using UnityEditor.Animations;*/
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(PhotonView))]
public class ThirdPersonInput :MonoBehaviour, IPunObservable
{
    [SerializeField] LayerMask noPlayerLayerMask;
    [SerializeField] CMS.Config.MapConfig roomScene;

    public PhotonView photonView;
    [SerializeField] float groundCheckRadius;

    public FloatingJoystick LeftJoystick;

    public FixedButton JumpButton;
    public FixedButton CrouchButton;
    public FixedTouchField TouchField;
    public float JumpForce = 5f;
    public float jumpHeight;
    public float rotateSpeed = 5f;
    public float cameraYSpeed = 5f;
    public float learpSpeedf =  1;
    public float distanceToForceTP = 1;
    protected ThirdPersonUserControl Control;

    [SerializeField] SkinsManager skinsManager;

    protected bool isCrouching = false;
    protected CapsuleCollider CapCollider;

    bool isGrounded;

    private bool hasReferencies = false;
    public float targetJump;

    [SerializeField] PlayerCamera camera;
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

        if(groundCheckRadius <= 0f)
        {
            groundCheckRadius = _characterController.radius;
        }

      //  mecanim.lookTarget = camera.lo
       // ObservedComponents.Add(this);
    }


    private void OnDestroy()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnectedAndReady)
        {
            if (Loader.Instance != null)
                Loader.Instance.AllSceneLoaded -= SubscibeToUI;


        }
    }

    private void Start()
    {

        if (!photonView.IsMine && PhotonNetwork.IsConnectedAndReady) {
            camera.gameObject.SetActive(false);
            m_Cam.gameObject.SetActive(false);
        }
        // get the third person character ( this should never be null due to require component )

    }



    private void FixedUpdate()
    {
        Collider[] overlapColliders =  Physics.OverlapSphere(transform.localPosition, groundCheckRadius, noPlayerLayerMask);

        isGrounded = overlapColliders.Length != 0 ? true : false;

        if (LeftJoystick != null && (photonView.IsMine || !PhotonNetwork.IsConnectedAndReady))
        {


            Hinput = Mathf.Clamp(LeftJoystick.input.x + Input.GetAxis("Horizontal"), -1, 1);
            Vinput = Mathf.Clamp(LeftJoystick.input.y + Input.GetAxis("Vertical"), -1, 1);

            if (JumpButton != null)
                m_Jump = (Input.GetKeyDown(KeyCode.Space) || JumpButton.Pressed);

            camera.MoveTo(TouchField.TouchDist.y * Time.fixedDeltaTime * -1);

            cameraRotation = new Vector3((transform.forward * 5 + transform.position).x, (transform.forward * 5 + transform.position).y + (camera.transform.forward * 5).y, (transform.forward * 5 + transform.position).z) + Vector3.up;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * TouchField.TouchDist.x * rotateSpeed * Time.fixedDeltaTime);

        }
        else if(newPosition != Vector3.zero)
        {
            Hinput = Mathf.Lerp(Hinput, newHinput, Time.fixedDeltaTime * learpSpeedf);
            Vinput = Mathf.Lerp(Vinput, newVinput, Time.fixedDeltaTime * learpSpeedf);
            targetJump = Mathf.Lerp(targetJump, newTargetJump, Time.fixedDeltaTime * learpSpeedf);

            if (Vector3.Distance(transform.position, newPosition) > distanceToForceTP) {

                transform.position = Vector3.Lerp(transform.position, newPosition, Time.fixedDeltaTime * learpSpeedf);

            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation),Time.fixedDeltaTime * learpSpeedf);
        }
        if (isGrounded)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = Vinput * transform.forward + Hinput * transform.right;
            mecanim.SetHorizontalSpeed(Hinput);
            mecanim.SetVerticalSpeed(Vinput);
            mecanim.SetJump(false);
            targetJump = -1f;
        }
        else
        {
/*            mecanim.SetHorizontalSpeed(0);
            mecanim.SetVerticalSpeed(0);*/
            mecanim.SetJump(true);
        }

        if (m_Move != Vector3.zero)
        {

            m_Move -= m_Move*Time.fixedDeltaTime;
        }



        Jump();

        /*mecanim.SetJump(targetJump);*/

        // pass all parameters to the character control script
        _characterController.Move(m_Move * Time.fixedDeltaTime * speed);
        m_Jump = false;

       
    }
    
    private void LateUpdate()
    {
        return;
        Vector3 rotation = Quaternion.LookRotation(cameraRotation - hips.transform.position , Vector3.up).eulerAngles;
        
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
/*        Debug.Log("IsMine " + photonView.IsMine);*/
        if ((photonView.IsMine || !PhotonNetwork.IsConnectedAndReady))
        {
            LeftJoystick = FindObjectOfType<FloatingJoystick>();
            TouchField = FindObjectOfType<FixedTouchField>();
            hasReferencies = true;
            if(!Loader.Instance.curentScene == roomScene)
            {
                JumpButton = FindObjectOfType<FixedButton>();
            }
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
        if (isGrounded && m_Jump)
        {
            targetJump = jumpHeight;
            inertionMovement = new Vector3(m_Move.x, targetJump, m_Move.z);

        }

        if (!isGrounded)
        {
            targetJump -=  Time.fixedDeltaTime * JumpForce;

            _characterController.Move(inertionMovement * Time.fixedDeltaTime);
        }
       /* else
        {
            targetJump = Mathf.Clamp(targetJump, 0, Mathf.Infinity);
            Debug.Log(_characterController.isGrounded);
        }*/

        m_Move.y = targetJump;
    }
    Vector3 newPosition = Vector3.zero;
    Vector3 newRotation = Vector3.zero;
    float newHinput = 0;
    float newVinput = 0;
    float newTargetJump = 0;

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
            stream.SendNext(targetJump);
        }
        else
        {
          //  Debug.Log("OnPhotonSerializeView IsReading");
            transform.localPosition = (Vector3)stream.ReceiveNext();
            transform.localRotation =  Quaternion.Euler((Vector3)stream.ReceiveNext());
            cameraRotation = (Vector3)stream.ReceiveNext();
           
            Hinput = (float)stream.ReceiveNext();
            Vinput = (float)stream.ReceiveNext();
            m_Jump = (bool)stream.ReceiveNext();
            newTargetJump = (float)stream.ReceiveNext();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.localPosition, 0.3f);
    }
}
