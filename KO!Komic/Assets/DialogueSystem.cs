using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class DialogueSystem : MonoBehaviour
{
    [Header("Config UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject joker;

    [Header("Config Speed")]
    public float charactersPerSecond = 30f; // Velocidade da digitação

    [Header("SFX")]
    public AudioClip characterSound;
    public AudioClip completeSound;
    public AudioClip openSound;
    public AudioClip closeSound;

    [Header("Dialogue List")]
    public List<DialogueLine> dialogueList;
    [SerializeField] int dialogueIndex;

    [Header("Config Tutorial")]
    public GameObject tutorialObj;
    public bool tutorial = false;

    AudioManager audioManager;
    bool isDialogueActive = false;
    bool isTyping = false;
    Coroutine typingCoroutine;

    [SerializeField] UIManager UIM;

    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(3, 5)]
        public string text;
        public AudioClip customSound;
        public Sprite jokerSprite;
        
    }

    void Start()
    {
        audioManager = GetComponentInChildren<AudioManager>();
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        //StartDialogue();
    }

    public void PlayerInput()
    {
        dialogueIndex++;

        print("foi foi dialogo1");

        if (!isDialogueActive) return;

        print("foi foi dialogo2");

        if (dialogueIndex > 13 && tutorial == false)
        {
            tutorialObj.SetActive(true);
            return;
        }

        if(dialogueIndex > dialogueList.Count)
        {
            UIM.ChangeToMenu();
        }

        if (GetComponent<GameMain>().training && GetComponent<GameMain>().fight)
        {
            if (isTyping)
            {
                // Skip typing animation
                SkipTyping();
            }
            else
            {
                // Advance to next line
                DisplayNextLine();
            }
        }

        // Skip entire dialogue
        //EndDialogue();
    }
    
    /// <summary>
    /// Inicia o diálogo
    /// </summary>
    public void StartDialogue()
    {
        print("inicia dialogo");

        gameObject.GetComponent<GameMain>().isPausing = true;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (openSound != null)
            audioManager.PlaySFX(openSound);

        DisplayNextLine();

    }

    /// <summary>
    /// Exibe a próxima linha do diálogo
    /// </summary>
    void DisplayNextLine()
    {
        print("passa pra proxima");

        if (dialogueIndex > dialogueList.Count && tutorial == true)
        {
            EndDialogue();
            return;
        }

        // Inicia animação de digitação
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogueList[dialogueIndex].text, dialogueList[dialogueIndex].customSound));
        StartCoroutine(FlipJoker());

        
    }

    /// <summary>
    /// Animação de texto letra por letra
    /// </summary>
    IEnumerator TypeText(string text, AudioClip customSound)
    {
        print("texto foi");

        isTyping = true;
        dialogueText.text = "";
        float delayBetweenChars = 1f / charactersPerSecond;

        foreach (char c in text)
        {
            dialogueText.text += c;

            float pitch = Random.Range(0.9f, 1.1f); // Variação de pitch
            audioManager.PlaySFX(customSound, 0.3f, pitch); // Volume reduzido

        }

        isTyping = false;
        if (completeSound != null)
            audioManager.PlaySFX(completeSound);

        yield return null;
    }

    IEnumerator FlipJoker()
    {
        joker.GetComponent<FlipAnimation>().newSprite = dialogueList[dialogueIndex].jokerSprite;

        joker.GetComponent<Animator>().SetTrigger("flip");

        yield return null;
    }

    /// <summary>
    /// Pula a animação de digitação atual
    /// </summary>
    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;

            dialogueText.text = dialogueList[dialogueIndex].text;

            if (completeSound != null)
                audioManager.PlaySFX(completeSound);
        }
    }

    /// <summary>
    /// Finaliza o diálogo
    /// </summary>
    void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(false);

        if (closeSound != null)
            audioManager.PlaySFX(closeSound);

        gameObject.GetComponent<GameMain>().isPausing = false;

        Debug.Log("Diálogo concluído!");
    }

    /// <summary>
    /// Pausa/Continua a digitação
    /// </summary>
    public void SetDialoguePaused(bool paused)
    {
        if (isTyping)
        {
            if (paused)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);
            }
            else
            {
                typingCoroutine = StartCoroutine(TypeText(dialogueList[dialogueIndex].text, dialogueList[dialogueIndex].customSound));
            }
        }
    }
}
