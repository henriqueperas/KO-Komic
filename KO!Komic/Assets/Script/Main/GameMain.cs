using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameMain : MonoBehaviour
{
    public GameObject[] characters;

    public GameObject player1;
    public GameObject player2;

    public HealthBar[] playerHealfBar;
    public ComboCounter[] playerCombo;

    GameObject playerWiner;

    Vector2 player1Posi = new Vector2(-2.2f, -4.85f);
    Vector2 player2Posi = new Vector2(2.2f, -4.85f);

    bool fight;

    public int playersReady = 0;
    int ready;

    public bool training;
    public bool arena;

    public bool isPausing;



    UIManager uim;
    MenuController mc;
    GameObject fc;



    // Start is called before the first frame update
    void Start()
    {
        uim = GameObject.Find("GameManager").GetComponent<UIManager>();
        mc = GameObject.Find("GameManager").GetComponent<MenuController>();
        fc = GameObject.Find("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<MenuController>().enabled = !fight;

        if (player1 == null && player2 == null) return;

        ready = (training || arena) ? 1 : 0;

        if ((playersReady + ready) >= 1)
        {
            CreatingCharacters();
        }

        if (fight)
        {
            EndMatch();
        }
        
    }

    void CreatingCharacters()
    {
        fc.GetComponent<FightingCamera>().enabled = true;
        playersReady = 0;

        var gamepads = Gamepad.all;

        TwoPlayerSetup tps = GetComponent<TwoPlayerSetup>();

        tps.CreatePlayer(gamepads[0], 1);


        if (gamepads.Count >= 2) // >= 2
        {
            tps.CreatePlayer(gamepads[1], 2);
        }
        else
        {
            player2 = characters[Random.Range(0, characters.Length)];
            player2 = Instantiate(player2, new Vector3(player2.transform.position.x + 10, player2.transform.position.y, player2.transform.position.z), Quaternion.identity);
            player2.GetComponent<PlayerInput>().enabled = false;
        }

        player1.GetComponent<PlayerController>().enemy = player2;
        player1.GetComponent<PlayerController>().enemytag = "AttackP2";
        player1.GetComponent<PlayerMain>().healthBar = playerHealfBar[0];
        player1.GetComponent<PlayerMain>().healthBar.player = player1.GetComponent<PlayerMain>();
        player1.GetComponent<PlayerMain>().cc = playerCombo[0];

        player2.GetComponent<PlayerController>().enemy = player1;
        player2.GetComponent<PlayerController>().enemytag = "AttackP1";
        player2.gameObject.tag = "Player2";
        player2.GetComponent<PlayerMain>().healthBar = playerHealfBar[1];
        player2.GetComponent<PlayerMain>().healthBar.player = player2.GetComponent<PlayerMain>();
        player2.GetComponent<PlayerMain>().cc = playerCombo[1];

        fight = true;

    }

    void EndMatch()
    {
        if (player1.GetComponent<PlayerMain>().health <= 0)
        {
            player2.GetComponent<PlayerMain>().wins++;
        }

        if (player1.GetComponent<PlayerMain>().wins >= 3)
        {
            fc.GetComponent<FightingCamera>().enabled = false;
            fc.transform.position = Vector3.zero;

            playerWiner = player1;
            Destroy(player1);

            uim.ChangeScreen(uim.endFight);
            mc.NewButton(uim.buttonEndFight);
            Destroy(player1);
            Destroy(player2);
            print("PLAYER 1 GANHOU");
        }

        if (player2.GetComponent<PlayerMain>().health <= 0)
        {
            player1.GetComponent<PlayerMain>().wins++;
        }

        if (player2.GetComponent<PlayerMain>().wins >= 3)
        {
            fc.GetComponent<FightingCamera>().enabled = false;
            fc.transform.position = Vector3.zero;

            playerWiner = player1;
            Destroy(player2);

            uim.ChangeScreen(uim.endFight);
            mc.NewButton(uim.buttonEndFight);
            Destroy(player1);
            Destroy(player2);
            print("PLAYER 2 GANHOU");
        }
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
        isPausing = false;
    }

    public void isTraining()
    {
        training = !training;

    }


}
