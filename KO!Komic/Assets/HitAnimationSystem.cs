using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimationSystem : MonoBehaviour
{
    PlayerController pc;
    Animator anim;

    bool fall;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
    }

    public void OnHit()
    {
        anim.SetTrigger("Hit");

        if (pc.air)
        {
            fall = true;
        }

        StartCoroutine(OnHitCorrotine());
    }

    public IEnumerator OnHitCorrotine()
    {
        pc.inHit = true;
        pc.canMove = false;
        yield return new WaitForFrames(5);
        if (fall && pc.isGrounded)
        {
            StartCoroutine(Recover(1f));
        }
        else
        {
            pc.canMove = true;
            pc.inHit = false;
        }
    }

    IEnumerator Recover(float time)
    {
        fall = false;
        yield return new WaitForSeconds(time);
        anim.SetTrigger("Recover");
    }
}
