using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{

    #region Configuration
    [Header("Hit Properties")]
    [Tooltip("Dano")]
    public float damage = 10f;
    [Tooltip("Delay para atacar novamente")]
    public float cooldown = 0.5f;

    [Header("Hit Box")]
    [Tooltip("Tamanho da HitBox (o Pivot por padrão fica no meio da HitBox)")]
    public Vector2 range;
    [Tooltip("Posição da HitBox em relação ao personagem")]
    public Vector2 position;

    [Header("Frame Data")]
    [Tooltip("Tempo que levará para a Hit Box ser ativada")]
    public int startupFrames;
    [Tooltip("Tempo que a Hit Box se mantem ativa")]
    public int activeFrames;
    [Tooltip("Tempo que leva para o personagem 'recolher' o golpe")]
    public int recoveryFrames;
    [Tooltip("Tempo de vantagem que o personagem ganha ao acertar um ataque diretamente")]
    public int hitAdvantage;
    [Tooltip("Tempo de vantagem quando o golpe é bloqueado (negativo quando for uma desvantagem)")]
    public int blockAdvantage;

    [Header("Inputs")]
    [Tooltip("Input do computador")]
    public KeyCode inputKey;  // Tecla de input  
    [Tooltip("Input do controle de Xbox")]
    public Inputs inputs;

    [Header("Sound")]
    [Tooltip("Som do golpe ao acertar o outro player")]
    public AudioSource hitSound;

    [Header("VFX")]
    [Tooltip("efeito ao acertar o outro player")]
    public ParticleSystem hitVFX;
    #endregion



    public enum Inputs
    {
        //----------KEYBORAD----------
        W,
        A,
        S,
        D,
        J,
        K,
        //----------XBOXONE----------
        buttonNorth,
        buttonSouth,
        buttonEast,
        buttonWest,
        up,
        down,
        right,
        left

    }

}
