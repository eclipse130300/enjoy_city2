using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsAnimator : MonoBehaviour
{
     public void AnimateBodyParts(Animator parentAnimator)
    {
        var allchildAnimators = GetComponentsInChildren<Animator>();
        foreach(Animator childAnim in allchildAnimators)
        {
            childAnim.runtimeAnimatorController = parentAnimator.runtimeAnimatorController;
            childAnim.avatar = parentAnimator.avatar;
        }

    }
}
