using System.Collections;
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
        print("foi");

        if(button)
        {
            currentScreen.SetActive(false);
            screen.SetActive(true);
            currentScreen = screen;
            button = false;
            StartCoroutine(cooldownButton());
        }
        
    }

    public void ChangeToMenu()
    {
        ChangeScreen(menu);
        gameObject.GetComponent<MenuController>().NewButton(buttonMenu);
        gameObject.GetComponent<GameMain>().am.PlayMusic(gameObject.GetComponent<GameMain>().m_menu);
    }

    IEnumerator cooldownButton()
    {
        yield return new WaitForSeconds(1f);
        button = true;
    }

    public void NewButton(GameObject backButton)
    {
        EventSystem.current.SetSelectedGameObject(backButton);

        gameObject.GetComponent<MenuController>().enabled = true;
    }

}
