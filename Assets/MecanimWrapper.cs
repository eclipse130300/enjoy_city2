using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanimWrapper : MonoBehaviour
{
    [SerializeField] Animator animator;
    public void SetHorizontalSpeed(float speed) {

        animator.SetFloat("MoveX", speed);
    }
    public void SetVerticalSpeed(float speed) {
        animator.SetFloat("MoveZ", speed);
    }
    public void SetJump(bool value)
    {
        animator.SetBool("Jump", value);
    }
}
