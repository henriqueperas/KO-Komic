using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class CombatSystem : MonoBehaviour
{
    [Header("Attack")]
    public AttackData[] attacks;  // Array de golpes 
    public AttackData[] attacksCrouched;  // Array de golpes 
    public AttackData[] attacksAir;  // Array de golpes 
    float[] nextAttackTime;  // Cooldowns individuais
    public Vector2 attackPosicionInspec;

    [Header("Combo")]
    [SerializeField] ComboData[] combos;  // Combos disponíveis
    [SerializeField] List<AttackData> currentComboSequence = new List<AttackData>();
    [SerializeField] float lastAttackTime;

    [SerializeField]  PlayerController player;

    [Header("Defender")]
    public bool block;
    public bool parry;

    Vector2 attackRangeInspec;
    public Animator anim;
    public bool timeRun;
    Transform attackPoint;
    Collider2D attackCollider;

    public bool canAttack = true;

    float timeCollisor = 0;

    // Start is called before the first frame update
    void Start()
    {
        nextAttackTime = new float[attacks.Length];
        anim = GetComponentInParent<Animator>();
        attackPoint = GetComponent<Transform>();
        attackCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        updateCollisor(attackRangeInspec, attackPosicionInspec);

        if (timeRun)
        {
            timeCollisor += Time.deltaTime;
        }
        

        if(timeCollisor >= 1f)
        {
            attackCollider.enabled = false;
        }

        // Reseta o combo se o jogador demorar muito
        if (Time.time - lastAttackTime > 0.75f && currentComboSequence.Count > 0)
        {
            ResetCombo();
        }
    }

    #region Hits and Combos
    public void TryAttack(int attackIndex, int attackType)
    {
        //print(attacks[attackIndex]);

        
        if (canAttack)
        {
            switch (attackType)
            {
                case 0:
                    player.attackCoroutine = StartCoroutine(OnStatup(attacks[attackIndex]));
                    //player.attackCoroutine = OnStatup(attacks[attackIndex]);
                    RegisterAttack(attacks[attackIndex]);
                    break;
                case 1:
                    player.attackCoroutine = StartCoroutine(OnStatup(attacksCrouched[attackIndex]));
                    RegisterAttack(attacksCrouched[attackIndex]);
                    break;
                case 2:
                    player.attackCoroutine = StartCoroutine(OnStatup(attacksAir[attackIndex]));
                    RegisterAttack(attacksAir[attackIndex]);
                    break;
            }
        }

    }

    public void RegisterAttack(AttackData attack)
    {
        currentComboSequence.Add(attack);
        lastAttackTime = Time.time;
        CheckCombos();
    }

    void CheckCombos()
    {
        foreach (ComboData combo in combos)
        {
            if (combo.sequence.Length != currentComboSequence.Count)
                continue;

            bool comboMatch = true;
            for (int i = 0; i < combo.sequence.Length; i++)
            {
                if (currentComboSequence[i] != combo.sequence[i])
                {
                    comboMatch = false;
                    break;
                }
            }

            if (comboMatch)
            {
                ExecuteFinisher(combo.finisher);
                ResetCombo();
                return;
            }
        }

        // Se a sequência for maior que o combo, reseta
        if (currentComboSequence.Count >= 4)
            ResetCombo();
    }

    void ExecuteFinisher(AttackData finisher)
    {
        //print(finisher);


        StartCoroutine(OnStatup(finisher));

        Debug.Log("Combo completo! Dano: " + finisher.damage);
    }

    void ResetCombo()
    {
        currentComboSequence.Clear();
    }

    void updateCollisor(Vector2 rande, Vector2 position)
    {
        gameObject.transform.localScale = rande;
        //gameObject.transform.position = position;
    }
    #endregion 

    public IEnumerator OnStatup(AttackData attack)
    {
        print(attack.name + " foi executado");
        // se levar golpe enquanto isso da um break (stop corrotina)
        canAttack = false;
        anim.SetTrigger(attack.name.ToString());
        yield return new WaitForFrames(attack.startupFrames);
        player.attackCoroutine = null;
        StartCoroutine(OnActivate(attack));
    }

    public IEnumerator OnActivate(AttackData attack)
    {
        attackRangeInspec = attack.range;
        attackPosicionInspec = attack.position;
        attackCollider.enabled = true;
        yield return new WaitForFrames(attack.activeFrames);
        attackCollider.enabled = false;
        StartCoroutine(OnRecovery(attack));
    }

    public IEnumerator OnRecovery(AttackData attack)
    {
        yield return new WaitForFrames(attack.recoveryFrames);
        canAttack = true;
    }



}
