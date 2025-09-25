using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAnimator : MonoBehaviour
{
    private Animator anim;
    PlayerController pc;
    private string currentState;

    bool fall;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
    }

    public void PlayAnimation(string newState)
    {
        // Evita interrupção da mesma animação
        if (currentState == newState) return;

        // Reseta todos os triggers ANTES de ativar o novo
        //anim.ResetAllTriggers();

        // Ativa o novo trigger
        anim.SetTrigger(newState);

        currentState = newState;
    }

    // Método para ataques básicos
    public void PlayAttack(int attackType)
    {
        anim.SetFloat("AttackType", attackType);
        if(attackType > 0)
        {
            anim.SetTrigger("Attack");
        }
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
        pc.cs.canAttack = true;
    }

    IEnumerator Recover(float time)
    {
        fall = false;
        yield return new WaitForSeconds(time);
        anim.SetTrigger("Recover");
        pc.cs.canAttack = true;
    }
}