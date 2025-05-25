using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    public string name;

    public HealthBar healthBar;

    public float health;
    public int maxHealth;

    public int wins;

    [SerializeField] ComboCounter cc;

    public void TakeDamage(int damage, bool block)
    {

        cc.AddCombo(1);

        if (block)
        {
            health -= ((float)damage * 0.5f);
        }
        else
        {
            health -= damage;
        }

        
        //health = Mathf.Max(0, health);

        //healthBar.isTakingDamage = true;
        healthBar.UpdateHealthBar();

        // Reseta o delay quando o personagem para de tomar dano
        //CancelInvoke(nameof(healthBar.ResetDamageFlag));
        //Invoke(nameof(healthBar.ResetDamageFlag), 0.5f); // Ajuste o tempo conforme necessário

        //healthBar.ResetDamageFlag();
    }

}
