using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject currentScreen;

    bool button = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void teste(int test)
    {
        print(test);
    }

    public void testf(int test) {
        print(test); 
    }

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

    public void CreatCharacter(GameObject test1, int test2)
    {

    }

    IEnumerator cooldownButton()
    {
        yield return new WaitForSeconds(1f);
        button = true;
    }

}
