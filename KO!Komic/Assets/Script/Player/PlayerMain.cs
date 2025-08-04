using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMain : MonoBehaviour
{
    public string name;

    public HealthBar healthBar;

    public float health;
    public int maxHealth;

    public int wins;

    [SerializeField] ComboCounter cc;

    UIManager uim;
    MenuController mc;
    GameMain gm;

    public bool inPause = false;

    private void Start()
    {
        uim = GameObject.Find("GameManager").GetComponent<UIManager>();
        mc = GameObject.Find("GameManager").GetComponent<MenuController>();
        gm = GameObject.Find("GameManager").GetComponent<GameMain>();
    }

    private void Update()
    {
        inPause = gm.isPausing;
    }

    public void TakeDamage(int damage, bool block)
    {

        cc.AddCombo(1);

        health -= block ? ((float)damage * 0.5f) : damage;

        
        health = Mathf.Max(0, health);

        healthBar.isTakingDamage = true;
        healthBar.UpdateHealthBar();

        // Reseta o delay quando o personagem para de tomar dano
        //CancelInvoke(nameof(healthBar.ResetDamageFlag));
        //Invoke(nameof(healthBar.ResetDamageFlag), 0.5f); // Ajuste o tempo conforme necessário

        //healthBar.ResetDamageFlag();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            uim.ChangeScreen(uim.pause);
            mc.NewButton(uim.buttonPause);
            gm.isPausing = true;
        }
    }

}
