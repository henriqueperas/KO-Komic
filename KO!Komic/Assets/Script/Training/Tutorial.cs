using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboDisplayText; // UI que mostra o combo atual
    [SerializeField] GameObject player;
    [SerializeField] GameMain gameMain;
    [SerializeField] DialogueSystem ds;
    int currentInputCorret = 0;

    public GameObject HUD;
    public GameObject tutorial;
    public GameObject gameManager;

    PlayerController pc;
    CombatSystem cs;

    string[] combo;
    public string comboInput;

    [SerializeField] int currentCombo;
    [SerializeField] int currentComboInput;

    // Start is called before the first frame update
    void Start()
    {
        player = gameMain.player1;

        pc = player.GetComponent<PlayerController>();
        cs = player.GetComponentInChildren<CombatSystem>();

        player.GetComponent<PlayerInput>().enabled = true;

        UpdateText();
    }

    private void Update()
    {
        if(currentCombo >= cs.combos.Length)
        {
            ds.tutorial = true;
            ds.PlayerInput();
            //gameObject.SetActive(false);
        }
    }

    public void UpdateText()
    {
        NewComboDisplay();

        string currentTextCombo = "";
        /*
        for (int i = 0; i < cs.combos[currentCombo].sequence.Length; i++)
        {
            if (i > cs.currentComboSequence.Count - 1)
            {
                return;
            }

            if (cs.currentComboSequence[i].inputs == cs.combos[currentCombo].sequence[i].inputs)
            {
                print("aqui foi");
                currentComboInput++;
                currentTextCombo += "<color=green>" + cs.combos[currentCombo].sequence[i].inputs + "</color>";
                comboInput = null;
            }
            else
            {
                print("aqui n foi");
                currentComboInput = 0;
                currentTextCombo += "<color=red>" + cs.combos[currentCombo].sequence[i].inputs + "</color>";
            }

            if (i < (cs.combos[currentCombo].sequence.Length - 1))
            {
                currentTextCombo += " + ";
            }
        }

        comboDisplayText.text += currentTextCombo;
        */

        for (int i = 0; i < cs.combos[currentCombo].sequence.Length; i++)
        {
            // Se o jogador ainda não digitou até este input
            if (i >= cs.currentComboSequence.Count)
            {
                // Mostra o restante em cor neutra (ainda não tentado)
                currentTextCombo += "<color=white>" + cs.combos[currentCombo].sequence[i].inputs + "</color>";
            }
            else
            {
                // Verifica se o input do jogador corresponde ao esperado
                if (cs.currentComboSequence[i].inputs == cs.combos[currentCombo].sequence[i].inputs)
                {
                    currentTextCombo += "<color=green>" + cs.combos[currentCombo].sequence[i].inputs + "</color>";
                    currentInputCorret++;
                }
                else
                {
                    currentTextCombo += "<color=red>" + cs.combos[currentCombo].sequence[i].inputs + "</color>";
                }
            }

            // Adiciona o separador "+" se não for o último input
            if (i < cs.combos[currentCombo].sequence.Length - 1)
            {
                currentTextCombo += " + ";
            }
        }

        comboDisplayText.text = currentTextCombo; // Atribui o texto completo

        if(currentInputCorret > cs.combos[currentCombo].sequence.Length)
        {
            currentCombo++;
        }

    }

    void NewComboDisplay()
    {
        comboDisplayText.text = "combo: " + cs.combos[currentCombo].name + "<br>";
    }
}
