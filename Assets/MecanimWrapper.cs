using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimWrapper : MonoBehaviour
{

    
    [SerializeField] Animator animator;

/*    private void Update()
    {
        SetHorizontalSpeed(0);
        SetVerticalSpeed(0);

    }*/

    public void SetHorizontalSpeed(float speed) {

        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var newAnimator in animators) {
            if (newAnimator.runtimeAnimatorController == null)
            {
                newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
            //    newAnimator.ForceStateNormalizedTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                newAnimator.avatar = animator.avatar;

            }
            if (newAnimator != animator) newAnimator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
          //  newAnimator.playbackTime = animator.playbackTime;
           

        }
        animator.SetFloat("MoveX", speed);
    }
    public void SetVerticalSpeed(float speed) {
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var newAnimator in animators)
        {
            if (newAnimator.runtimeAnimatorController == null)
            {
                newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
              //  newAnimator.ForceStateNormalizedTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                newAnimator.avatar = animator.avatar;
            }
           if(newAnimator != animator) newAnimator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //newAnimator.playbackTime = animator.playbackTime;
            
        }
        animator.SetFloat("MoveZ", speed);
    }
    public void SetJump(float value)
    {
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var newAnimator in animators)
        {
            if (newAnimator.runtimeAnimatorController == null)
            {
                newAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
             //   newAnimator.ForceStateNormalizedTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                newAnimator.avatar = animator.avatar;
            }
            if (newAnimator != animator) newAnimator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            
        }
        animator.SetFloat("Jump", value*0.8f);
    }
}
