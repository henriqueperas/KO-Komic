using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public enum AIState { Idle, Chase, Attack, Block }
public class EnemyAI : MonoBehaviour
{
    [Header("Config")]
    public float chaseDistance = 10f;
    public float attackDistance = 5f;
    public float decisionInterval = 2f;
    public bool canAttack = false;
    public int speed = 3;

    [Header("Referências")]
    public Transform player;
    public CombatSystem cs;
    Animator anim;
    //public ComboData[] availableCombos; // Seus ScriptableObjects de combo

    [SerializeField] AIState currentState;
    private float nextDecisionTime = 3f;

    public bool canFight = false;

    private void Start()
    {
        player = GameObject.Find("GameManager").GetComponent<GameMain>().player1.transform;
        cs = GetComponentInChildren<CombatSystem>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (Time.time >= nextDecisionTime && canFight)
        {
            canAttack = true;
            DecideNextAction();
            print("IA aqui indo");
            nextDecisionTime = Time.time + decisionInterval;
        }

        if (canFight == false)
        {
            if(GetComponent<PlayerMain>().maxHealth > GetComponent<PlayerMain>().health)
            {
                GetComponent<PlayerMain>().health++;
            }
        }



        ExecuteCurrentState();
    }

    void DecideNextAction()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        print(distanceToPlayer);
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
        cs.TryAttack(Random.Range(1, cs.attacks.Length), 0);
        //AttackData attack = availableAttacks[Random.Range(0, availableAttacks.Length)];
        // Executa um ataque único

        //currentState = AIState.Idle;
    }
}
