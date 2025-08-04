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
            fc.GetComponent<FightingCamera>().enabled = true;
            playersReady = 0;

            
            

            var gamepads = Gamepad.all;
            print("aqui foi 1");
            TwoPlayerSetup tps = GetComponent<TwoPlayerSetup>();
            print("aqui foi 2");
            tps.CreatePlayer(gamepads[0], 1);
            print("aqui foi 3");

            if (gamepads.Count >= 4) // >= 2
            {
                tps.CreatePlayer(gamepads[1], 2);
            }
            else
            {
                player2 = characters[Random.Range(0, characters.Length)];
                player2 = Instantiate(player2);
                player2.GetComponent<PlayerInput>().enabled = false;
            }

            

            player1.GetComponent<PlayerController>().enemy = player2;
            player1.GetComponent<PlayerController>().enemytag = "AttackP2";

            player2.GetComponent<PlayerController>().enemy = player1;
            player2.GetComponent<PlayerController>().enemytag = "AttackP1";
            player2.gameObject.tag = "Player";

            playersReady = -10;
        }

        if(playersReady == -10) 
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
