using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 15f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool crouched = false;
    public bool air = false;

    [Header("Enemy")]
    public GameObject enemy;
    public string enemytag;

    [Header("Combat")]
    public CombatSystem cs;
    public Coroutine attackCoroutine;
    public bool inHit = false;

    [Header("Jump States")]
    public bool isRising;       // Subindo
    public bool isFalling;      // Caindo
    public bool reachedPeak;    // Pico do pulo
    private float previousYVelocity; // Armazena a velocidade vertical do frame anterior

    [SerializeField] Tutorial tu;
    
    FightingCamera fc;

    [SerializeField] AudioManager am;

    public int playerID;
    public bool p2;

    Rigidbody2D rb;
    public Vector2 moveInput;
    public bool isGrounded = true;
    [SerializeField] bool canJump = true;
    float timeJump = 0.5f;
    Animator anim;
    PlayerMain main;

    public int modMoveAnim;

    Vector3 hitNormal;

    public bool canMove = true;
    public bool canAttack = true;

    bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //cs = GetComponent<CombatSystem>();
        anim = GetComponent<Animator>();
        main = GetComponent<PlayerMain>();

        fc = GameObject.Find("MainCamera").GetComponent<FightingCamera>();

        anim.SetBool("isGrounded", isGrounded);

        
    }

    public void StartPlayer()
    {
        if (enemy.transform.position.x > gameObject.transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            //cs.gameObject.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            modMoveAnim = 1;

            hitNormal = new Vector3(0, 0, 0);
        }
        else
        {
            //GetComponent<SpriteRenderer>().flipX = true;
            //cs.gameObject.transform.position = new Vector2 (-gameObject.transform.position.x, gameObject.transform.position.y);
            //gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-gameObject.GetComponent<BoxCollider2D>().offset.x,gameObject.GetComponent<BoxCollider2D>().offset.y);

            gameObject.transform.localScale = new Vector2(-1, 1);
            cs.p2 = true;
            cs.gameObject.tag = "AttackP2";
            modMoveAnim = -1;

            hitNormal = new Vector3(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {     
        anim.SetFloat("Speed", moveInput.x * modMoveAnim);
        anim.SetBool("Air", air);
        anim.SetBool("Crounch", crouched);

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.525f, groundLayer);
        air = !isGrounded;
        canJump = isGrounded;

        UpdateJumpStates();

        

        if(hit && air)
        {
            canMove = false;
        }
        else if (hit && !air && !canMove)
        {
            StartCoroutine(RecoverCoolDown());
        }
        

    }

    private void FixedUpdate()
    {
        if (!main.inPause && canMove)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
        // animação de movimento
    }

    #region Iputs
    public void OnMove(InputAction.CallbackContext context)
    {
        print(context.control.name.ToString());

        if (canMove && !main.inPause)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        print(context.control.name.ToString());
        if (context.performed && isGrounded && canJump && !main.inPause)
        {
            canJump = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // animação do pulo
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //talvez fazer um outro OnAttack que ao invez de chamar o inputKey chama o inputXbox que vai ter um string (não queria mas é o que temos) respectivo a cada movimento
        //print(context.control.name.ToString());
        //tu.comboInput = context.control.name.ToString();
        //tu.input = true;
        if (context.performed && !main.inPause && canAttack)
        {
            if(crouched == false && air == false)
            {
                // Verifica qual golpe foi solicitado
                for (int i = 0; i < cs.attacks.Length; i++)
                {
                    if (context.control.name.ToString() == (cs.attacks[i].inputs.ToString()))
                    {
                        
                        cs.TryAttack(i, 0);
                        canAttack = false;
                        StartCoroutine(CoolDownAttack(cs.attacks[i].cooldown));
                        break;
                    }
                }
            }
            else if (crouched == true && air == false)
            {

                // Verifica qual golpe foi solicitado
                for (int i = 0; i < cs.attacksCrouched.Length; i++)
                {
                    
                    if (context.control.name.ToString() == (cs.attacksCrouched[i].inputs.ToString()))
                    {
                        cs.TryAttack(i, 1);
                        canAttack = false;
                        StartCoroutine(CoolDownAttack(cs.attacksCrouched[i].cooldown));
                        break;
                    }
                }
            }
            else if (crouched == false && air == true)
            {
                // Verifica qual golpe foi solicitado
                for (int i = 0; i < cs.attacksAir.Length; i++)
                {

                    if (context.control.name.ToString() == (cs.attacksAir[i].inputs.ToString()))
                    {
                        cs.TryAttack(i, 2);
                        canAttack = false;
                        StartCoroutine(CoolDownAttack(cs.attacksAir[i].cooldown));
                        break;
                    }
                }
            }

        }
    }

    public void OnSquat(InputAction.CallbackContext context)
    {
        print(context.control.name.ToString());
        if (context.performed && isGrounded && !main.inPause)
        {
            //cc.AddCombo(1);
            //gameObject.transform.localScale = new Vector2(1f, 0.5f);
            GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 2f);
            GetComponent<BoxCollider2D>().size = new Vector2(2f, 4f);
            crouched = true;
            canMove = false;
        }
        else
        {
            //gameObject.transform.localScale = new Vector2(1f, 1f);
            GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f,2.75f);
            GetComponent<BoxCollider2D>().size = new Vector2(2f, 5.5f);
            crouched = false;
            canMove = true;
        }
        
    }

    public void OnBlockParry(InputAction.CallbackContext context)
    {
        if (context.performed && air == false && !main.inPause)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("block", true);
            cs.canAttack = false;
            canJump = false;
            canMove = false;
            cs.block = true;
            
        }
        else
        {
            anim.SetBool("block", false);
            cs.canAttack = true;
            canJump = true;
            canMove = true;
            cs.block = false;
            
        }
    }

    #endregion

    void UpdateJumpStates()
    {
        float currentYVelocity = GetComponent<Rigidbody2D>().velocity.y;

        if (!isGrounded)
        {
            // Subindo (velocidade Y positiva e aumentando)
            isRising = currentYVelocity > -3.991578f && currentYVelocity > previousYVelocity;
        }

        // Pico do pulo (velocidade Y próxima de zero)
        reachedPeak = Mathf.Abs(currentYVelocity) < 0.1f && !isGrounded;

        // Caindo (velocidade Y negativa ou diminuindo)
        isFalling = (currentYVelocity < 0 || (currentYVelocity > 0 && currentYVelocity < previousYVelocity)) && !isGrounded;

        previousYVelocity = currentYVelocity; // Atualiza para o próximo frame

        anim.SetBool("IsRising", isRising);
        anim.SetBool("IsFalling", isFalling);
        anim.SetBool("ReachedPeak", reachedPeak);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == enemytag)
        {
            fc.TriggerHitEffect();
            //StartCoroutine(OnHit());
            print($" {enemytag} ta batendo");
            if (cs.parry)
            {
                print("TA PORRA UM PARRY");
                return;
            }
            if (!cs.block)
            {
                gameObject.GetComponent<FighterAnimator>().OnHit();

                hit = true;

                if (cs.p2)
                {
                    rb.AddForce(new Vector2(5 * 100, 1 * 50));
                }
                else
                {
                    rb.AddForce(new Vector2(5 * 100, 1 * 50));
                }
            }
            //anim.SetTrigger("Hit");
            //cc.AddCombo(1);
            main.TakeDamage(collision.gameObject.GetComponent<CombatSystem>().damage, cs.block);


            //PONTUACAO

            collision.gameObject.GetComponentInParent<PlayerMain>().score = (int)collision.gameObject.GetComponent<CombatSystem>().damage * 15;




            Vector3 midPoint = new Vector3((transform.position.x + collision.transform.position.x) / 2f, collision.transform.position.y, collision.transform.position.z);
            Vector3 hitNormal = (collision.transform.position - transform.position).normalized;

            SpawnVFX(midPoint, hitNormal, collision.gameObject.GetComponent<CombatSystem>().vfx);

            am.PlaySFX(cs.sfx);

            if (collision.gameObject.GetComponent<PlayerMain>().health > 0 && attackCoroutine != null) {
                StopCoroutine(attackCoroutine);
            }
            
        }
    }

    void SpawnVFX(Vector3 position, Vector3 normal, GameObject HitVFX)
    {
        if (HitVFX == null) return;

        // Instancia o VFX na posição da colisão
        GameObject vfxInstance = Instantiate(HitVFX, position, Quaternion.identity);

        // Rotaciona o VFX para ficar perpendicular à superfície
        vfxInstance.transform.rotation = Quaternion.LookRotation(Vector3.zero);

        vfxInstance.GetComponent<ParticleSystem>().Play();

        // Destroi após um tempo
        Destroy(HitVFX, 5f);
    }

    IEnumerator Parry()
    {
        cs.parry = true;
        yield return new WaitForFrames(5);
        cs.parry = false;
    }
    IEnumerator CoolDownAttack(float time)
    {
        yield return new WaitForFrames((int)time);
        canAttack = true;
    }

    public IEnumerator OnHit()
    {
        inHit = true;
        yield return new WaitForFrames(5);
        inHit = false;
    }

    IEnumerator RecoverCoolDown()
    {
        yield return new WaitForFrames(45);
        canMove = true;
        hit = false;
    }

}
