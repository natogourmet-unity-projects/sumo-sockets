using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour {

    public Animator animator;
    public bool isWalking;

    public void SetAnimation()
    {
        animator.SetBool("isWalking", isWalking);
        isWalking = !isWalking;
    }
}
