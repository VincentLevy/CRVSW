using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//anim control on the prefab
public class animControl : MonoBehaviour
{
    public Animator animator;

    public void SetIsAttacking(bool value)
    {
        animator.SetBool("isAttacking", value);
    }
}
