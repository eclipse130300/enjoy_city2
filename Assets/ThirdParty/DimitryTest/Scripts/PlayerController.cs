using System;
using System.Collections;
using System.Collections.Generic;
using SocialGTA;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private float targetJump;
    private Vector3 inertionMovement;


    private CharacterController _characterController;
    [Range(0.01f,1f)]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform target; //move relative to target(camera)
    [SerializeField] private float jumpHeight;
    

    // Use this for initialization
    void Start()
    {
        if (target == null) target = GameObject.FindWithTag("PlayerCamera").transform;
        _animator = GetComponentInChildren<CharacterSkinModel>().animator;
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;
        float horizontal = Input.GetAxis("Vertical");
        float vertical = Input.GetAxis("Horizontal");

        if ((horizontal != 0 || vertical != 0) && _characterController.isGrounded )
        {
            _animator.SetFloat("MoveZ", 1f);
            
            movement.z = horizontal * movementSpeed;
            movement.x = vertical * movementSpeed;
            movement = Vector3.ClampMagnitude(movement, movementSpeed);

            Quaternion startOrient = target.rotation;
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement);
            target.rotation = startOrient;

            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotationSpeed);
        }

        movement.y = targetJump;
        targetJump -= Time.deltaTime * 5;
       

        if (_characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            targetJump = jumpHeight;
            inertionMovement = new Vector3(movement.x, targetJump, movement.z);
            _animator.SetTrigger("Jump");
        }

        if (!_characterController.isGrounded)
        {
            _characterController.Move(inertionMovement * Time.deltaTime);
        }

        inertionMovement -= inertionMovement * (Time.deltaTime * 3);
        movement *= Time.deltaTime;
        _characterController.Move(movement);
        
        
    }
}
