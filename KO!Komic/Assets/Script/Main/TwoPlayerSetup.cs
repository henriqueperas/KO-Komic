using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class TwoPlayerSetup : MonoBehaviour
{
    GameMain gm;

    void Start()
    {
        gm = gameObject.GetComponent<GameMain>();
    }

    public void CreatePlayer(Gamepad device, int playerID)
    {
        PlayerInput playerInput = (playerID == 1) ?
            PlayerInput.Instantiate(gm.player1.GetComponent<PlayerInput>()) :
            PlayerInput.Instantiate(gm.player1.GetComponent<PlayerInput>());

        if(playerID == 1)
        {
            gm.player1 = playerInput.gameObject;
        }
        else
        {
            gm.player2 = playerInput.gameObject;
        }

        print("cu");

        playerInput.SwitchCurrentControlScheme("Gamepad", device);
        playerInput.GetComponent<PlayerController>().playerID = playerID; // Identifica o jogador
    }

}

