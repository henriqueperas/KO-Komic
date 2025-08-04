using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] PlayerMain player;
    public float delayDecreaseSpeed = 20f; // Velocidade da barra vermelha

    [Header("Referências UI")]
    public Image healthBarFill; // Barra verde
    public Image healthBarDelay; // Barra vermelha

    public bool isTakingDamage;
    private float targetHealthNormalized;

    void Start()
    {
        player.healthBar = GetComponent<HealthBar>();
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        targetHealthNormalized = player.health / player.maxHealth;
        targetHealthNormalized *= healthBarDelay.transform.localScale.x;
        healthBarFill.fillAmount -= targetHealthNormalized; // Atualiza instantaneamente
        healthBarFill.transform.localScale = new Vector3(targetHealthNormalized, healthBarFill.transform.localScale.y, 0);
    }

}
