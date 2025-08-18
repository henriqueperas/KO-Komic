using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TwoPlayerSetup : MonoBehaviour
{
    GameMain gm;

    void Start()
    {
        gm = gameObject.GetComponent<GameMain>();
    }

    public void CreatePlayer(Gamepad device, int playerID)
    {/*
        PlayerInput playerInput = (playerID == 1) ?
            PlayerInput.Instantiate(gm.player1.GetComponent<PlayerInput>()) :
            PlayerInput.Instantiate(gm.player1.GetComponent<PlayerInput>());
        */

        PlayerInput playerInput;

        if (playerID == 1)
        {
            playerInput = PlayerInput.Instantiate(gm.player1.GetComponent<PlayerInput>(), new Vector3(gm.player1.transform.position.x, gm.player1.transform.position.y, gm.player1.transform.position.z), Quaternion.identity);
            gm.player1 = playerInput.gameObject;
        }
        else
        {
            if(playerID != 0)
            {
                playerInput = PlayerInput.Instantiate(gm.player2.GetComponent<PlayerInput>(), new Vector3(gm.player2.transform.position.x + 10, gm.player2.transform.position.y, gm.player2.transform.position.z), Quaternion.identity);
            }
            else
            {
                playerInput = PlayerInput.Instantiate(gm.player1.GetComponent<PlayerInput>(), new Vector3(gm.player1.transform.position.x + 10, gm.player1.transform.position.y, gm.player1.transform.position.z), Quaternion.identity);
            }
            gm.player2 = playerInput.gameObject;
        }

        playerInput.SwitchCurrentControlScheme("Gamepad", device);
        playerInput.GetComponent<PlayerController>().playerID = playerID; // Identifica o jogador
    }

}

