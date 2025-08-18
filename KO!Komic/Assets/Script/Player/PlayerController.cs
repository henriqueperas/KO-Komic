using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] CombatSystem cs;
    public Coroutine attackCoroutine;
    public bool inHit = false;

    [Header("Jump States")]
    public bool isRising;       // Subindo
    public bool isFalling;      // Caindo
    public bool reachedPeak;    // Pico do pulo
    private float previousYVelocity; // Armazena a velocidade vertical do frame anterior

    [SerializeField] Tutorial tu;
    
    FightingCamera fc;

    public int playerID;

    Rigidbody2D rb;
    public Vector2 moveInput;
    public bool isGrounded = true;
    [SerializeField] bool canJump = true;
    float timeJump = 0.5f;
    Animator anim;
    PlayerMain main;

    public bool canMove = true;

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

    // Update is called once per frame
    void Update()
    {        
        anim.SetFloat("Speed", moveInput.x);
        anim.SetBool("Air", air);
        anim.SetBool("Crounch", crouched);

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.525f, groundLayer);
        air = !isGrounded;
        canJump = isGrounded;

        UpdateJumpStates();

        // animações no chão

        if (enemy.transform.position.x > gameObject.transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            cs.gameObject.transform.position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y);
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
            cs.gameObject.transform.position = new Vector2 (gameObject.transform.position.x * -1, gameObject.transform.position.y);
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
        if (context.performed && !main.inPause)
        {
            if(crouched == false && air == false)
            {
                // Verifica qual golpe foi solicitado
                for (int i = 0; i < cs.attacks.Length; i++)
                {
                    if (context.control.name.ToString() == (cs.attacks[i].inputs.ToString()))
                    {
                        
                        cs.TryAttack(i, 0);

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
        }
        else
        {
            //gameObject.transform.localScale = new Vector2(1f, 1f);
            GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f,2.75f);
            GetComponent<BoxCollider2D>().size = new Vector2(2f, 5.5f);
            crouched = false;
        }
        
    }

    public void OnBlockParry(InputAction.CallbackContext context)
    {
        if (context.performed && air == false && !main.inPause)
        {
            anim.SetBool("block", true);
            cs.canAttack = false;
            canJump = false;
            canMove = false;
            cs.block = true;
            cs.parry = true;
        }
        else
        {
            anim.SetBool("block", false);
            cs.canAttack = true;
            canJump = true;
            canMove = true;
            cs.block = false;
            cs.parry = false;
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
            gameObject.GetComponent<HitAnimationSystem>().OnHit();
            //anim.SetTrigger("Hit");
            //cc.AddCombo(1);
            main.TakeDamage(5, cs.block); //MUDAR DEPOIS
            
            rb.AddForce(new Vector2(5 * 100, 1 * 50));

            StopCoroutine(attackCoroutine);
        }
    }

    public IEnumerator OnHit()
    {
        inHit = true;
        yield return new WaitForFrames(5);
        inHit = false;
    }



}
