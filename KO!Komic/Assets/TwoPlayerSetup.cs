using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class TwoPlayerSetup : MonoBehaviour
{
    [SerializeField] bool p2;

    [SerializeField] private PlayerInput player1Prefab;
    [SerializeField] private PlayerInput player2Prefab;

    void Start()
    {
        var gamepads = Gamepad.all;
        if (gamepads.Count == 1) // >= 2
        {
            CreatePlayer(gamepads[0], 1);
            if (p2)
            {
                CreatePlayer(gamepads[1], 2);
            }
        }
        else
        {
            Debug.LogError("Conecte 2 controles de Xbox!");
        }
    }

    void CreatePlayer(Gamepad device, int playerID)
    {
        PlayerInput playerInput = (playerID == 1) ?
            PlayerInput.Instantiate(player1Prefab) :
            PlayerInput.Instantiate(player2Prefab);

        print("test");

        playerInput.SwitchCurrentControlScheme("Gamepad", device);
        playerInput.GetComponent<PlayerController>().playerID = playerID; // Identifica o jogador
    }
}

