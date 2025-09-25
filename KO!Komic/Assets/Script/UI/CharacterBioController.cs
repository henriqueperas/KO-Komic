using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBioController : MonoBehaviour
{
    public GameObject[] cba;

    public List<string> attacks = new List<string>();

    public void UpdateAttacksList(CombatSystem cs)
    {
        attacks.Clear();

        for (int i = 0; i < cs.combos.Length; i++)
        {
            attacks.Add(cs.combos[i].name);
        }
        for (int i = 1; i < cs.attacks.Length; i++)
        {
            attacks.Add(cs.attacks[i].name);
        }
        for (int i = 0; i < cs.attacksCrouched.Length; i++)
        {
            attacks.Add(cs.attacksCrouched[i].name);
        }
        for (int i = 0; i < cs.attacksAir.Length; i++)
        {
            attacks.Add(cs.attacksAir[i].name);
        }
        

        UpdateAttackText();
    }


    public void UpdateAttackText()
    {
        for (int i = 0;i < cba.Length; i++)
        {
            cba[i].GetComponent<CharacterBioAttacks>().textAttack.text = attacks[cba[i].GetComponent<CharacterBioAttacks>().indexPreview];
        }
    }
}
