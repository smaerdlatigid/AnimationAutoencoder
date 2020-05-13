using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLoop : MonoBehaviour
{
    Animator animator; 
    public List<string> animations = new List<string>();
    public int i = 0;

    void Start()
    {
        animator = GetComponent<Animator>();

        AnimationClip[] arrclip = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in arrclip)
        {
            animations.Add(clip.name);
        }

        animator.Play(animations[i]);
    }

    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            i++;
            i = i%animations.Count;
            animator.Play(animations[i]);
        }
    }
}
