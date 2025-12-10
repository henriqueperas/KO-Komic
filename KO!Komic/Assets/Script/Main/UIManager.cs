using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject currentScreen;

    public GameObject pause;
    public GameObject buttonPause;
    public GameObject endFight;
    public GameObject buttonEndFight;
    public GameObject menu;
    public GameObject buttonMenu;

    bool button = true;

    public void ChangeScreen(GameObject screen)
    {
        print(screen);

        screen.SetActive(true);
        currentScreen.SetActive(false);
        currentScreen = screen;

    }

    public void ChangeToMenu()
    {
        ChangeScreen(menu);
        gameObject.GetComponent<MenuController>().NewButton(buttonMenu);
        gameObject.GetComponent<GameMain>().am.PlayMusic(gameObject.GetComponent<GameMain>().m_menu);
    }

}
