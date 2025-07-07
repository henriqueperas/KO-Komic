using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public enum AIState { Idle, Chase, Attack, Block }
public class EnemyAI : MonoBehaviour
{
    [Header("Config")]
    public float chaseDistance;
    public float attackDistance;
    public float decisionInterval;
    public bool canAttack;
    public int speed;

    [Header("Referências")]
    public Transform player;
    public CombatSystem cs;
    Animator anim;
    //public ComboData[] availableCombos; // Seus ScriptableObjects de combo

    private AIState currentState;
    private float nextDecisionTime;


    private void Start()
    {
        //cs = GetComponent<CombatSystem>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            canAttack = true;
            DecideNextAction();
            nextDecisionTime = Time.time + decisionInterval;
        }

        ExecuteCurrentState();
    }

    void DecideNextAction()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackDistance)
        {
            currentState = Random.Range(0, 100) < 70 ? AIState.Attack : AIState.Block;
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Idle;
        }
    }

    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                // Fica parado ou faz uma animação de idle
                break;

            case AIState.Chase:
                anim.SetFloat("Speed", 1);
                Vector3 direction = (player.position - transform.position).normalized;
                transform.Translate(direction * speed * Time.deltaTime);
                break;

            case AIState.Attack:
                if (Random.Range(0, 100) < 30) // 30% de chance de fazer combo
                {
                    StartCoroutine(ExecuteRandomCombo());
                }
                else
                {
                    ExecuteRandomAttack();
                }
                break;

            case AIState.Block:
                // Lógica de defesa
                break;
        }
    }

    IEnumerator ExecuteRandomCombo()
    {
        //print("teste combo");
        canAttack = false;
        yield return new WaitForSeconds(0.5f);
        //ComboData combo = availableCombos[Random.Range(0, availableCombos.Length)];
        /*
        foreach (var input in combo.inputSequence)
        {
            // Simula o input do botão
            SimulateInput(input);
            yield return new WaitForSeconds(combo.inputDelay);
        }*/
    }

    void ExecuteRandomAttack()
    {
        canAttack = false;
        //print(cs.attacks[Random.Range(0, cs.attacks.Length)].name);
        cs.TryAttack(Random.Range(0, (cs.attacks.Length - 4)), 0);
        //AttackData attack = availableAttacks[Random.Range(0, availableAttacks.Length)];
        // Executa um ataque único
    }
}
