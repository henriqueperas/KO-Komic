using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Attack")]
    public AttackData[] attacks;  // Array de golpes 
    public AttackData[] attacksCrouched;  // Array de golpes 
    public AttackData[] attacksAir;  // Array de golpes 
    float[] nextAttackTime;  // Cooldowns individuais
    public Vector2 attackPosicionInspec;
    [SerializeField] Vector2 attackRangeInspec;

    [Header("Combo")]
    public ComboData[] combos;  // Combos disponíveis
    public List<AttackData> currentComboSequence = new List<AttackData>();
    [SerializeField] float lastAttackTime;

    [SerializeField] PlayerController player;

    [SerializeField] GameObject playerObj;

    [Header("Defender")]
    public bool block;
    public bool parry;

    public bool p2;
    
    //public Animator anim;
    public FighterAnimator anim;
    public bool timeRun;
    Transform attackPoint;
    Collider2D attackCollider;

    public bool canAttack = true;

    public float damage;
    public GameObject vfx;
    public AudioClip sfx;

    float timeCollisor = 0;

    // Start is called before the first frame update
    void Start()
    {
        nextAttackTime = new float[attacks.Length];
        //anim = GetComponentInParent<Animator>();
        anim = GetComponentInParent<FighterAnimator>();
        attackPoint = GetComponent<Transform>();
        attackCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        updateCollisor(attackRangeInspec, attackPosicionInspec);

        if (timeRun)
        {
            timeCollisor += Time.deltaTime;
        }
        
        /*
        if(timeCollisor >= 1f)
        {
            attackCollider.enabled = false;
        }
        */
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
                    //player.attackCoroutine = StartCoroutine(OnStatup(attacks[attackIndex]));
                    //player.attackCoroutine = OnStatup(attacks[attackIndex]);
                    RegisterAttack(attacks[attackIndex]);
                    break;
                case 1:
                    //player.attackCoroutine = StartCoroutine(OnStatup(attacksCrouched[attackIndex]));
                    RegisterAttack(attacksCrouched[attackIndex]);
                    break;
                case 2:
                    //player.attackCoroutine = StartCoroutine(OnStatup(attacksAir[attackIndex]));
                    RegisterAttack(attacksAir[attackIndex]);
                    break;
            }
        }

    }

    public void RegisterAttack(AttackData attack)
    {
        currentComboSequence.Add(attack);
        
        lastAttackTime = Time.time;
        CheckCombos(attack);

        if (GameObject.Find("GameManager").gameObject.GetComponent<GameMain>().training == true)
        {
            GameObject.Find("GameManager").gameObject.GetComponent<GameMain>().tutorial.UpdateText();
        }
    }

    void CheckCombos(AttackData attack)
    {
        bool comboMatch = false;
        foreach (ComboData combo in combos)
        {
            if (combo.sequence.Length != currentComboSequence.Count)
                continue;

            comboMatch = true;
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


        player.attackCoroutine = StartCoroutine(OnStatup(attack));

        // Se a sequência for maior que o combo, reseta
        if (currentComboSequence.Count >= 4)
            ResetCombo();
    }

    void ExecuteFinisher(AttackData finisher)
    {
        //print(finisher);


        StartCoroutine(OnStatup(finisher));

        //Debug.Log("Combo completo! Dano: " + finisher.damage);
    }

    void ResetCombo()
    {
        currentComboSequence.Clear();
    }

    void updateCollisor(Vector2 rande, Vector2 position)
    {
        //gameObject.transform.localScale = rande;
        //gameObject.transform.position = position;
        //attackCollider.offset = position;
        //transform.position = position;

        
        if (player.p2)
        {
            //attackCollider.offset = new Vector2(-position.x, position.y);
            //transform.position = new Vector2(playerObj.transform.position.x + position.x, playerObj.transform.position.y + position.y);

            transform.position = new Vector2(playerObj.transform.position.x + (position.x * -1), playerObj.transform.position.y + position.y);
        }
        else
        {
            transform.position = new Vector2(playerObj.transform.position.x + position.x, playerObj.transform.position.y + position.y);
        }
        GetComponent<BoxCollider2D>().size = rande;
    }
    #endregion 

    public IEnumerator OnStatup(AttackData attack)
    {
        damage = attack.damage;
        vfx = attack.hitVFX;
        sfx = attack.audioClip;
        //print(attack.name + " foi executado");
        // se levar golpe enquanto isso da um break (stop corrotina)
        canAttack = false;
        //anim.SetTrigger(attack.name.ToString());
        anim.PlayAttack(attack.animType);
        player.attackCoroutine = null;
        attackRangeInspec = attack.range;
        attackPosicionInspec = attack.position;
        yield return new WaitForFrames(attack.startupFrames);
        StartCoroutine(OnActivate(attack));
    }

    public IEnumerator OnActivate(AttackData attack)
    {
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
