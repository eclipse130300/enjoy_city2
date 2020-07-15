using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimWrapper : MonoBehaviour
{

    
    public Animator animator;
    public Transform lookTarget;
    public Transform rightLeg;
    public Transform leftLeg;
    public bool ikActive = true;
    public LayerMask groundMask;
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
        Debug.Log(value);
        animator.SetBool("Jumping", value);
        

        animator.SetLayerWeight(0, value ? 0 : 1);
        animator.SetLayerWeight(1, value ? 1 : 0);
        /*        SetBoolParameterForAllChildren("Jump", value, animators);*/
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
}
