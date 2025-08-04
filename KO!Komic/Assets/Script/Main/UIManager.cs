using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject currentScreen;

    public GameObject pause;
    public GameObject buttonPause;
    public GameObject endFight;
    public GameObject buttonEndFight;

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

    IEnumerator cooldownButton()
    {
        yield return new WaitForSeconds(1f);
        button = true;
    }

}
