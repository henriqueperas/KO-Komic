using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombo", menuName = "Combat/Combo Data")]
public class ComboData : ScriptableObject
{
    [Header("Sequência")]
    [Tooltip("Sequencia de golpes para dar o golpe final")]
    public AttackData[] sequence;  // Ex: [Soco, Soco, Chute]

    [Header("Golpe Final")]
    [Tooltip("Golpe final")]
    public AttackData finisher;    // Ataque final
    [Tooltip("Multiplicado de Dano")]
    public float damageMultiplier = 1.5f;  // Dano extra

    [Header("Timing")]
    [Tooltip("Delay para o proximo golpe")]
    public float maxDelayBetweenAttacks = 0.3f;  // Tempo máximo entre golpes

    [Header("Specal Attack")]
    [Tooltip("Indicador de ataque especial")]
    public bool special;
    public SpecialAttack specialAttack;


}
