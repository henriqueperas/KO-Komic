using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 15f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool crouched = false;
    [SerializeField] bool air = false;

    [Header("Enemy")]
    [SerializeField] GameObject enemy;
    [SerializeField] string enemytag;

    [Header("Combat")]
    [SerializeField] CombatSystem cs;
    public Coroutine attackCoroutine;

    [Header("Jump States")]
    public bool isRising;       // Subindo
    public bool isFalling;      // Caindo
    public bool reachedPeak;    // Pico do pulo
    private float previousYVelocity; // Armazena a velocidade vertical do frame anterior

    [SerializeField] ComboCounter cc;
    [SerializeField] Tutorial tu;

    public int playerID;

    Rigidbody2D rb;
    Vector2 moveInput;
    bool isGrounded = true;
    [SerializeField] bool canJump = true;
    float timeJump = 0.5f;
    Animator anim;
    PlayerMain main;

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //cs = GetComponent<CombatSystem>();
        anim = GetComponent<Animator>();
        main = GetComponent<PlayerMain>();

        anim.SetBool("isGrounded", isGrounded);
    }

    // Update is called once per frame
    void Update()
    {        
        anim.SetFloat("Speed", moveInput.x);

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.525f, groundLayer);
        air = !isGrounded;
        canJump = isGrounded;

        UpdateJumpStates();

        // animações no chão

        if (enemy.transform.position.x > gameObject.transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            cs.gameObject.transform.position = new Vector2
                (cs.attackPosicionInspec.x + gameObject.transform.position.x
                , cs.attackPosicionInspec.y + gameObject.transform.position.y);
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
            cs.gameObject.transform.position = new Vector2
                ((cs.attackPosicionInspec.x * -1) + gameObject.transform.position.x
                , cs.attackPosicionInspec.y + gameObject.transform.position.y);
        }

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        // animação de movimento
    }

    #region Iputs
    public void OnMove(InputAction.CallbackContext context)
    {
        print(context.control.name.ToString());

        if (canMove)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        print(context.control.name.ToString());
        if (context.performed && isGrounded && canJump)
        {
            print("pula pula");
            canJump = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // animação do pulo
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //talvez fazer um outro OnAttack que ao invez de chamar o inputKey chama o inputXbox que vai ter um string (não queria mas é o que temos) respectivo a cada movimento
        print(context.control.name.ToString());
        //tu.comboInput = context.control.name.ToString();
        //tu.input = true;
        if (context.performed)
        {
            if(crouched == false && air == false)
            {
                //print("penis");
                // Verifica qual golpe foi solicitado
                for (int i = 0; i < cs.attacks.Length; i++)
                {
                    if (context.control.name.ToString() == (cs.attacks[i].inputs.ToString()))
                    {
                        //print("penis penis");
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
                    print("penis penis");
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

                    print("piroca piroca");
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
        if (context.performed && isGrounded)
        {
            anim.SetBool("Crounch", true);
            cc.AddCombo(1);
            //gameObject.transform.localScale = new Vector2(1f, 0.5f);
            crouched = true;
        }
        else
        {
            anim.SetBool("Crounch", false);
            //gameObject.transform.localScale = new Vector2(1f, 1f);
            crouched = false;
        }
        
    }

    public void OnBlockParry(InputAction.CallbackContext context)
    {
        if (context.performed && air == false)
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

        // Subindo (velocidade Y positiva e aumentando)
        isRising = currentYVelocity > -3.991578f && currentYVelocity > previousYVelocity;

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
            print(enemytag + " ta batendo");
            anim.SetTrigger("Hit");
            //cc.AddCombo(1);
            main.TakeDamage(5, cs.block); //MUDAR DEPOIS
            
            rb.AddForce(new Vector2(5 * 100, 1 * 50));
            
            StopCoroutine(attackCoroutine);
        }
    }



}
