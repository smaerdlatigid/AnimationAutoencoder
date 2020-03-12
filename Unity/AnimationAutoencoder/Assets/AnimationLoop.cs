using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLoop : MonoBehaviour
{
    Animator animator; 
    public string[] animations;
    public int i = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play(animations[i]);
    }

    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            i++;
            i = i%animations.Length;
            animator.Play(animations[i]);
        }
    }
}
