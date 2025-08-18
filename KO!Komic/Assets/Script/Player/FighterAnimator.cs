using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAnimator : MonoBehaviour
{
    private Animator anim;
    private string currentState;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
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
}