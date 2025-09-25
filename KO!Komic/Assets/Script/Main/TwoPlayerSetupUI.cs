using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

public class TwoPlayerSetupUI : MonoBehaviour
{
    bool p1 = true;
    bool p2 = true;

    public GameObject objPlayer1UI;
    public GameObject objPlayer1EventSystem;
    public GameObject objPlayer2UI;
    public GameObject objPlayer2EventSystem;

    public GameObject btPlayer1Start;
    public GameObject btPlayer2Start;

    public GameObject eventSystemGeral;

    public MenuController mc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StarUI2Player()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2) // >= 2
        {
            mc.enabled = false;

            //eventSystemGeral.SetActive(false);

            objPlayer1EventSystem.GetComponent<MyEventSystem>().firstSelectedGameObject = btPlayer1Start;
            objPlayer2EventSystem.GetComponent<MyEventSystem>().firstSelectedGameObject = btPlayer2Start;

            objPlayer1UI.SetActive(true);
            objPlayer1EventSystem.SetActive(true);
            objPlayer2UI.SetActive(true);
            objPlayer2EventSystem.SetActive(true);

            if (p1)
            {
                //executa para o p1
                CreatePlayerForUI(gamepads[1], objPlayer1UI, objPlayer1EventSystem);
                p1 = false;
            }

            if (gamepads.Count >= 2 && p2) // >= 2
            {
                //player2 = characters[0];
                CreatePlayerForUI(gamepads[1], objPlayer2UI, objPlayer2EventSystem);
                p2 = false;
            }
        }

    }

    public void EndUI2Player()
    {
        mc.enabled = true;

        //eventSystemGeral.SetActive(true);

        objPlayer1UI.SetActive(false);
        objPlayer1EventSystem.SetActive(false);
        objPlayer2UI.SetActive(false);
        objPlayer2EventSystem.SetActive(false);

    }


    void CreatePlayerForUI(Gamepad device, GameObject objPlayer, GameObject PlayerEventSystem)
    {
        PlayerInput playerInput;

        playerInput = PlayerInput.Instantiate(objPlayer.GetComponent<PlayerInput>());

        playerInput.SwitchCurrentControlScheme("Gamepad", device);
        PlayerEventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset = playerInput.actions;
    }
}
