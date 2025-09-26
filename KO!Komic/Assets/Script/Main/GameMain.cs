using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMain : MonoBehaviour
{
    [Header("Sound")]
    public AudioManager am;
    public AudioClip m_menu;
    public AudioClip m_fight;

    [Header("Characters")]
    public GameObject[] characters;

    [Header("Configuration Start Fight")]
    public GameObject player1;
    public GameObject player2;
    [SerializeField] int fightTimer;
    [SerializeField] float fightCurrentTime = 0;
    [SerializeField] TextMeshProUGUI fightTimerText;

    [Header("Conf of Slow Motion")]
    [SerializeField] private float slowDownFactor = 0.2f;     // Velocidade durante slow motion
    [SerializeField] private float slowDownDuration = 5f;   // Duração do slow motion
    [SerializeField] private float returnDuration = 0.5f;     // Tempo para voltar ao normal
    [SerializeField] private AnimationCurve slowMotionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Players Conf")]
    public HealthBar[] playerHealfBar;
    public ComboCounter[] playerCombo;
    public int playersReady = 0;
    public int ready;

    [Header("Musics")]
    public AudioSource[] menus;

    [Header("States")]
    public bool training;
    public bool arena;
    public bool isPausing;
    public bool fight;

    [Header("HUDS")]
    public GameObject trainingScreen;
    public Tutorial tutorial;

    [Header("Hightcore")]
    public HighscoreInput hi;

    GameObject playerWiner;

    Vector2 player1Posi = new Vector2(-2.2f, -4.85f);
    Vector2 player2Posi = new Vector2(2.2f, -4.85f);

    [SerializeField] GameObject lastPlayer1;
    [SerializeField] GameObject lastPlayer2;

    UIManager uim;
    MenuController mc;
    GameObject fc;

    // Start is called before the first frame update
    void Start()
    {
        uim = gameObject.GetComponent<UIManager>();
        mc = gameObject.GetComponent<MenuController>();
        fc = GameObject.Find("MainCamera");
        am = gameObject.GetComponentInChildren<AudioManager>();

        am.PlayMusic(m_menu);

        fightCurrentTime = fightTimer;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (player1 == null || player2 == null) return;

        //ready = (training || arena) ? 1 : 0;
        

        if ((playersReady + ready) >= 1)
        {
            CreatingCharacters();
           // print(playersReady + ready);
        }

        if (fight )
        {
            
            EndMatch();
            FightTime();
        }

        

    }

    public void CreatingCharacters()
    {
        lastPlayer1 = player1;

        //gameObject.GetComponent<MenuController>().enabled = !fight;
        fc.GetComponent<FightingCamera>().enabled = true;
        playersReady = 0;

        var gamepads = Gamepad.all;

        TwoPlayerSetup tps = GetComponent<TwoPlayerSetup>();

        tps.CreatePlayer(gamepads[0], 1);

        print(gamepads.Count);

        if (gamepads.Count >= 2) // >= 2
        {
            //player2 = characters[0];
            lastPlayer2 = player2;
            tps.CreatePlayer(gamepads[1], 2);
        }
        else
        {
            player2 = characters[0];
            lastPlayer2 = player2;
            player2 = Instantiate(player2, new Vector3(player2.transform.position.x + 10, player2.transform.position.y, player2.transform.position.z), Quaternion.identity);
            player2.GetComponent<PlayerInput>().enabled = false;
        }

        print("teste");

        print(gamepads.Count);


        //PrepareCharacter();

        GetComponent<Loading>().LoadSceneWithSmartLoading();


    }

    public void PrepareCharacter()
    {
        am.PlayMusic(m_fight);

        player1.GetComponent<PlayerController>().enemy = player2;
        player1.GetComponent<PlayerController>().enemytag = "AttackP2";
        player1.GetComponent<PlayerMain>().healthBar = playerHealfBar[0];
        player1.GetComponent<PlayerMain>().healthBar.player = player1.GetComponent<PlayerMain>();
        player1.GetComponent<PlayerMain>().cc = playerCombo[0];

        player2.GetComponent<PlayerController>().enemy = player1;
        player2.GetComponent<PlayerController>().enemytag = "AttackP1";
        player2.gameObject.tag = "Player2";
        player2.GetComponent<PlayerController>().p2 = true;
        player2.GetComponent<PlayerMain>().healthBar = playerHealfBar[1];
        player2.GetComponent<PlayerMain>().healthBar.player = player2.GetComponent<PlayerMain>();
        player2.GetComponent<PlayerMain>().cc = playerCombo[1];

        player1.GetComponent<PlayerController>().enabled = true;
        player1.GetComponent<PlayerMain>().enabled = true;
        player1.GetComponent<FighterAnimator>().enabled = true;

        player2.GetComponent<PlayerController>().enabled = true;
        player2.GetComponent<PlayerMain>().enabled = true;
        player2.GetComponent<FighterAnimator>().enabled = true;

        player1.GetComponent<PlayerController>().StartPlayer();
        player2.GetComponent<PlayerController>().StartPlayer();

        isPausing = false;
        fight = true;

        mc.enabled = true;
    }

    void EndMatch()
    {
        if (player1.GetComponent<PlayerMain>().health <= 0)
        {
            player2.GetComponent<PlayerMain>().wins++;
            StartCoroutine(SlowMotionRoutine());
        }
        else if (player2.GetComponent<PlayerMain>().health <= 0)
        {
            player1.GetComponent<PlayerMain>().wins++;
            StartCoroutine(SlowMotionRoutine());
        }

        if (player1.GetComponent<PlayerMain>().wins >= 3)
        {
            
            hi.enabled = true;

            fc.GetComponent<FightingCamera>().enabled = false;
            //fc.transform.position = Vector3.zero;

            playerWiner = player1;

            StartCoroutine(SlowMotionRoutine());

            print("PLAYER 1 GANHOU");

            
            

            hi.scoreText.text = playerWiner.GetComponent<PlayerMain>().score.ToString();
        } 
        else  if (player2.GetComponent<PlayerMain>().wins >= 3)
        {

            
            hi.enabled = true;

            //hi.UISetScore();

            fc.GetComponent<FightingCamera>().enabled = false;
            //fc.transform.position = Vector3.zero;

            playerWiner = player2;

            StartCoroutine(SlowMotionRoutine());

            print("PLAYER 2 GANHOU");

            
            
            hi.scoreText.text = playerWiner.GetComponent<PlayerMain>().score.ToString();
        }
    }

    public void EndFight()
    {
        

        if (playerWiner != null)
        {
            am.StopMusic();

            fc.transform.position = new Vector3(0, 0, -3);

            uim.ChangeScreen(uim.endFight);
            mc.NewButton(uim.buttonEndFight);
            Destroy(player1);
            Destroy(player2);
            player1 = null;
            player2 = null;

            hi.enabled = true;
        }
    }

    public void FightTime()
    {
        fightCurrentTime -= 1 * Time.deltaTime;

        fightTimerText.text = fightCurrentTime.ToString("0");

        if (fightCurrentTime <= 0)
        {
            if (player1.GetComponent<PlayerMain>().health > player2.GetComponent<PlayerMain>().health)
            {
                player2.GetComponent<PlayerMain>().health = 0;
            }
            else
            {
                player1.GetComponent<PlayerMain>().health = 0;
            }
        }
    }

    public void ReStart()
    {
        if(player1 != null && player2 != null)
        {
            Destroy(player1);
            Destroy(player2);
            player1 = null;
            player2 = null;
        }

        fightCurrentTime = fightTimer;
        player1 = lastPlayer1;
        player2 = lastPlayer2;

        GetComponent<Loading>().LoadClear();

        playerWiner = null;

        fc.transform.position = new Vector3(0f, 0f, fc.transform.position.z);
    }

    public void ResetCamera()
    {
        //fc.GetComponent<FightingCamera>().enabled = true;
        
    }

    public void SpawnCharac1(int type)
    {
        player1 = Instantiate(characters[type]);
        player1.transform.position = player1Posi;

        //MUDAR DEPOIS, SISTEMA MAIS COMPLEXO PARA SABER QUANDO ESTÁ EM LUTA
        fight = true;
    }

    public void OutPause()
    {
        fight = !fight;
        isPausing = false;
    }

    public void isTraining()
    {
        training = !training;

    }

    public IEnumerator SlowMotionRoutine()
    {
        // Fase 1: Entrada no slow motion (rápido)
        yield return StartCoroutine(EnterSlowMotion(0.1f));

        // Fase 2: Mantém o slow motion
        yield return new WaitForSecondsRealtime(slowDownDuration);

        // Fase 3: Volta ao normal (suave)
        yield return StartCoroutine(ExitSlowMotion(returnDuration));
    }

    private IEnumerator EnterSlowMotion(float duration)
    {
        float timer = 0f;
        float startTimeScale = Time.timeScale;
        float startFixedDelta = Time.fixedDeltaTime;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / duration;
            float curveProgress = slowMotionCurve.Evaluate(progress);

            // Interpola suavemente para o slow motion
            Time.timeScale = Mathf.Lerp(startTimeScale, slowDownFactor, curveProgress);
            //Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

            yield return null;
        }

        // Garante valores exatos no final
        Time.timeScale = slowDownFactor;
        //Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
    }

    /// <summary>
    /// Saída gradual do slow motion
    /// </summary>
    private IEnumerator ExitSlowMotion(float duration)
    {
        float timer = 0f;
        float startTimeScale = Time.timeScale;
        float startFixedDelta = Time.fixedDeltaTime;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / duration;
            float curveProgress = slowMotionCurve.Evaluate(progress);

            // Interpola suavemente de volta ao normal
            Time.timeScale = Mathf.Lerp(startTimeScale, 1f, curveProgress);
            //Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

            yield return null;
        }

        // Garante valores exatos no final
        Time.timeScale = 1f;
        //Time.fixedDeltaTime = originalFixedDeltaTime;
        fc.transform.position = new Vector3(0, 0, -3);

        yield return new WaitForSeconds(0.1f);

        EndFight();
    }


}
