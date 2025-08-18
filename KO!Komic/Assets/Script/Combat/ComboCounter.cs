using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    [Header("Sprites dos Números")]
    public Sprite[] numberSprites; // Array com sprites de 0 a 9

    [Header("Referências UI")]
    public Image[] digitImages; // Imagens dos dígitos (Digit1, Digit2...)
    public GameObject comboBalloon;  // Imagem do balão

    public int currentCombo = 0;

    public float time = 5;

    void Start()
    {
        UpdateComboDisplay();
    }

    void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            ResetCombo();
            time = 5f;
        }

        if(currentCombo <= 0)
        {
            comboBalloon.SetActive(false);
        }
        else
        {
            comboBalloon.SetActive(true);
        }
    }

    public void AddCombo(int points)
    {
        time = 5f;
        currentCombo += points;
        UpdateComboDisplay();
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        UpdateComboDisplay();
    }

    void UpdateComboDisplay()
    {
        string comboStr = currentCombo.ToString();

        // Desativa todos os dígitos inicialmente
        foreach (var img in digitImages)
        {
            img.gameObject.SetActive(false);
        }

        // Ativa e atualiza apenas os dígitos necessários
        for (int i = 0; i < comboStr.Length; i++)
        {
            if (i >= digitImages.Length) break; // Evita overflow

            int digit = int.Parse(comboStr[i].ToString());
            digitImages[i].sprite = numberSprites[digit];
            digitImages[i].gameObject.SetActive(true);
        }

        // Ativa/desativa o balão conforme o combo

        if(currentCombo > 0)
        {
            comboBalloon.SetActive(true);
        }
        else
        {
            comboBalloon.SetActive(false);
        }

    }
}
