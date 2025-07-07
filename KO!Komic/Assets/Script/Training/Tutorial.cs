using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboDisplayText; // UI que mostra o combo atual
    [SerializeField] GameObject player;
    [SerializeField] GameMain gameMain;

    PlayerController pc;
    CombatSystem cs;

    string[] combo;
    public string comboInput;

    [SerializeField] int currentCombo;
    [SerializeField] int currentComboIput;
    public bool input;



    // Start is called before the first frame update
    void Start()
    {
        pc = player.GetComponent<PlayerController>();
        cs = player.GetComponentInChildren<CombatSystem>();
        UpdateText(currentCombo);

    }

    // Update is called once per frame
    void Update()
    {
        if(gameMain.training = true && input == true)
        {
            UpdateText(currentCombo);
            input = false;
        }

        if(currentComboIput >= cs.combos[currentCombo].sequence.Length)
        {
            currentComboIput = 0;
            currentCombo++;
        }
    }

    void UpdateText(int comboIndex)
    {
        comboDisplayText.text = "combo: " + cs.combos[comboIndex].name + "<br>";
        for(int i = 0; i < cs.combos[comboIndex].sequence.Length; i++)
        {

            if(comboInput == cs.combos[comboIndex].sequence[i].inputs.ToString())
            {
                print("aqui foi");
                currentComboIput++;
                comboDisplayText.text += "<color=green>" + cs.combos[comboIndex].sequence[i].inputs + "</color>";
                comboInput = null;
            }
            else
            {
                print("aqui n foi");
                currentComboIput = 0;
                comboDisplayText.text += "<color=red>" + cs.combos[comboIndex].sequence[i].inputs + "</color>";
            }

            if(i < (cs.combos[comboIndex].sequence.Length - 1))
            {
                comboDisplayText.text += " + ";
            }
        }
    }
}
