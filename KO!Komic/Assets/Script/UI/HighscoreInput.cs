using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighscoreInput : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text nameText; // Mostra "AAA"
    public TMP_Text scoreText; // Mostra a pontuação
    private int currentCharIndex = 0;
    private char[] initials = { 'A', 'A', 'A' }; // Inicia com AAA
    public LeaderboardSystem ls;

    [Header("Input")]
    public InputActionReference navigateAction; // D-Pad/analógico
    public InputActionReference confirmAction; // Botão A
    public InputActionReference cycleLettersAction; // Gatilhos/botões laterais

    private void OnEnable()
    {
        // Configura os inputs
        navigateAction.action.performed += OnNavigate;
        confirmAction.action.performed += OnConfirm;
        cycleLettersAction.action.performed += OnCycleLetters;

        navigateAction.action.Enable();
        confirmAction.action.Enable();
        cycleLettersAction.action.Enable();
    }

    private void OnDisable()
    {
        navigateAction.action.performed -= OnNavigate;
        confirmAction.action.performed -= OnConfirm;
        cycleLettersAction.action.performed -= OnCycleLetters;
    }

    // Troca a letra selecionada para cima e baixo
    void OnCycleLetters(InputAction.CallbackContext ctx)
    {
        float input = ctx.ReadValue<float>();
        initials[currentCharIndex] = (char)(initials[currentCharIndex] + (input > 0 ? 1 : -1));

        // Limita às letras A-Z
        if (initials[currentCharIndex] < 'A') initials[currentCharIndex] = 'Z';
        if (initials[currentCharIndex] > 'Z') initials[currentCharIndex] = 'A';

        UpdateNameDisplay();
    }

    // Navega entre as letras para os lados
    void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input.x > 0.5f) currentCharIndex = Mathf.Min(currentCharIndex + 1, 2);
        else if (input.x < -0.5f) currentCharIndex = Mathf.Max(currentCharIndex - 1, 0);

        UpdateNameDisplay();
        //ls.ResetLeaderboard();
    }

    // Confirma o nome (Botão A)
    void OnConfirm(InputAction.CallbackContext ctx)
    {
        ls.AddHighscore(new string(initials), int.Parse(scoreText.text));

        print("FOI");
        // Carrega próxima cena ou volta ao menu
    }

    void UpdateNameDisplay()
    {
        nameText.text = new string(initials);
        // Destaque visual da letra selecionada (opcional)
        nameText.text = $"<color=#FF0000>{initials[0]}</color>{initials[1]}{initials[2]}";

        nameText.text =
            (currentCharIndex == 0 ? $"<color=red>{initials[0]}</color>" : initials[0].ToString()) +
            (currentCharIndex == 1 ? $"<color=red>{initials[1]}</color>" : initials[1].ToString()) +
            (currentCharIndex == 2 ? $"<color=red>{initials[2]}</color>" : initials[2].ToString());
    }
}



