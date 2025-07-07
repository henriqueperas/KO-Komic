using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Canvas buttonCanvas;

    public int characID;
    public string characName;

    [SerializeField] GameObject characPreview;
    Animator anim;
    [SerializeField] TextMeshProUGUI name;

    private void Start()
    {
        anim = characPreview.GetComponent<Animator>();

        buttonCanvas = GetComponent<Canvas>();
        if (buttonCanvas == null)
        {
            buttonCanvas = gameObject.AddComponent<Canvas>();
            buttonCanvas.overrideSorting = true;
            buttonCanvas.sortingOrder = 0; // Valor padrão
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        anim.SetInteger("type", characID);
        name.text = characName;
        buttonCanvas.sortingOrder = 1; // Coloca para frente
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonCanvas.sortingOrder = 0; // Coloca para frente
    }
}
