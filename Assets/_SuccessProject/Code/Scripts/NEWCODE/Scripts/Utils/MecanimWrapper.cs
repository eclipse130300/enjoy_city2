using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MecanimWrapper : MonoBehaviour, IOnEventCallback
{

    public Animator animator;
    public Transform lookTarget;
    public Transform rightLeg;
    public Transform leftLeg;
    public Transform leftHand;
    public bool ikActive = true;
    public LayerMask groundMask;

    public Vector3 leftHandGoal;

    public void SetHorizontalSpeed(float speed) {
        if (animator == null) return;

        Animator[] animators = GetComponentsInChildren<Animator>(false);
        foreach (var newAnimator in animators) {
            if (newAnimator.runtimeAnimatorController == null)
            {
                newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
            //    newAnimator.ForceStateNormalizedTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                newAnimator.avatar = animator.avatar;

            }
            /*if (newAnimator != animator)*/ newAnimator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
          //  newAnimator.playbackTime = animator.playbackTime;
           
        }
        animator.SetFloat("MoveX", speed);
/*        SetFloatParameterForAllChildren("MoveX", speed, animators);*/
    }
    public void SetVerticalSpeed(float speed) {
        if (animator == null) return;

        Animator[] animators = GetComponentsInChildren<Animator>(false);
        foreach (var newAnimator in animators)
        {
            if (newAnimator.runtimeAnimatorController == null)
            {
                newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
              //  newAnimator.ForceStateNormalizedTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                newAnimator.avatar = animator.avatar;
            }
           /*if(newAnimator != animator)*/ newAnimator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //newAnimator.playbackTime = animator.playbackTime;
            
        }
        animator.SetFloat("MoveZ", speed);
/*        SetFloatParameterForAllChildren("MoveZ", speed, animators);*/
    }
    public void SetJump(bool value)
    {

        if (animator == null) return;
        Animator[] animators = GetComponentsInChildren<Animator>(false);
        foreach (var newAnimator in animators)
        {
            if (newAnimator.runtimeAnimatorController == null)
            {
                newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
             //   newAnimator.ForceStateNormalizedTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                newAnimator.avatar = animator.avatar;
            }
            /*if (newAnimator != animator)*/ newAnimator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            
        }

        animator.SetBool("Jumping", value);


        animator.SetLayerWeight(0, value ? 0 : 1);
        animator.SetLayerWeight(1, value ? 1 : 0);
    }

    public void SetDeadState(bool value)
    {
        animator.SetLayerWeight(2, value == true ? 0f : 1f);
        animator.SetLayerWeight(1, value ? 1f : 0f);

        animator.SetBool("isDead", value);
    }

    public void StartFire() //todo refactor as soon as we have animations
    {
        animator.SetBool("isFiring", true);
    }

    public void EndFire()
    {
        animator.SetBool("isFiring", false);
    }

    public void Reload()
    {
        animator.SetTrigger("reload");
    }

    public void SetLayerWeight(int layer, float value)
    {
        animator.SetLayerWeight(layer, value);
    }

    public void SetFloatParameterForAllChildren(string parameterName, float value, Animator[] children)
    {
        foreach (Animator child in children)
        {
            child.SetFloat(parameterName, value);
        }
    }

    public void SetBoolParameterForAllChildren(string parameterName, bool value, Animator[] children)
    {
        foreach (Animator child in children)
        {
            child.SetBool(parameterName, value);
        }
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                /*                if (getGroundPos(transform.position, Vector3.down) == Vector3.zero) return;*/

                // Set the look target position, if one has been assigned
                if (lookTarget != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookTarget.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (leftLeg != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);   
                    
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, getGroundPos( leftLeg.position,Vector3.down));
                }
                if (rightLeg != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, getGroundPos(rightLeg.position, Vector3.down));
                }
                if(leftHand != null && leftHandGoal != Vector3.zero)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGoal);
                    Debug.Log(leftHandGoal);

                }


            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
    public Vector3 getGroundPos(Vector3 from,Vector3 direction, float offset=0.1f, float distance = 4) {
        RaycastHit hit;
        if (Physics.Raycast(from, direction, out hit, distance, groundMask)) {
          
            return hit.point + hit.normal * offset;
        
        }
        return Vector3.zero;

    }

    public void SetLeftHandIKGoal(Vector3 IKgoldWorldCoordinates)
    {
        leftHandGoal = IKgoldWorldCoordinates;
        Debug.Log("SetLeftHandIKGoal");
    }

    public void DisposeLeftHandIKGoal()
    {
        leftHandGoal = Vector3.zero;
        Debug.Log("DisposeLeftHandIKGoal");
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GameEvents.PLAYER_RESPAWNED)
        {
            animator.SetLayerWeight(2, 0f);
        }
    }
}
