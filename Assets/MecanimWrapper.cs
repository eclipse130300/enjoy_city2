using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimWrapper : MonoBehaviour
{

    
    public Animator animator;

/*    private void Update()
    {
        SetHorizontalSpeed(0);
        SetVerticalSpeed(0);

    }*/

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


}
