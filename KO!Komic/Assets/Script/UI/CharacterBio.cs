using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBio : MonoBehaviour
{
    SelectableButton sb;

    [SerializeField] RuntimeAnimatorController animController;

    [SerializeField] CharacterBioController cbc;

    [SerializeField] GameObject characterPreview;

    [SerializeField] GameMain gm;
    [SerializeField] int character;

    public CombatSystem cs;

    bool select = true;

    // Start is called before the first frame update
    void Start()
    {
        sb = GetComponent<SelectableButton>();

        cs = gm.characters[character].GetComponentInChildren<CombatSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        if (sb == null)
        {
            return;
        }
        else if (sb.selected && select)
        {
            characterPreview.GetComponent<Animator>().runtimeAnimatorController = animController;

            cbc.UpdateAttacksList(cs);

            characterPreview.GetComponent<Animator>().enabled = false;
            select = false;
        }else if(!sb.selected && !select)
        {
            select = true;
        }
    }

}
