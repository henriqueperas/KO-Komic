using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBioAttacks : MonoBehaviour
{
    SelectableButton sb;

    [SerializeField] GameObject characterPreview;

    public int indexPreview;

    public TextMeshProUGUI textAttack;
    [SerializeField] TextMeshProUGUI textAttackBio;

    [SerializeField] CharacterBioController cbc;

    // Start is called before the first frame update
    void Start()
    {
        sb = GetComponent<SelectableButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sb == null)
        {
            return;
        }
        else if (sb.selected)
        {
            characterPreview.GetComponent<Animator>().enabled = true;
            characterPreview.GetComponent<Animator>().SetFloat("Index", (indexPreview + 1));
        }
    }
}
