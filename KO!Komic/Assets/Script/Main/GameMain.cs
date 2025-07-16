using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public GameObject[] characters;

    GameObject player1;
    GameObject player2;

    Vector2 player1Posi = new Vector2(-2.2f, -4.85f);
    Vector2 player2Posi = new Vector2(2.2f, -4.85f);

    bool fight;

    public bool training;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<MenuController>().enabled = !fight;
    }

    public void SpawnCharac1(int type)
    {
        player1 = Instantiate(characters[type]);
        player1.transform.position = player1Posi;

        //MUDAR DEPOIS, SISTEMA MAIS COMPLEXO PARA SABER QUANDO ESTÁ EM LUTA
        fight = true;
    }

    public void SpawnCharac2(int type)
    {

    }

    public void isTraining()
    {
        training = !training;

    }


}
