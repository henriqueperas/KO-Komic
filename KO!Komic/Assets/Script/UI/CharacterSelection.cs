using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Canvas buttonCanvas;

    public int characID;
    public string characName;

    [SerializeField] bool p1;

    public float moveSpeed = 1f;

    [SerializeField] GameObject characPreview;
    Animator anim;
    [SerializeField] TextMeshProUGUI name;

    private RectTransform rectTransform;

    [SerializeField] CharacterSelectionController controller;

    private Vector2 originalPosition;

    private void Start()
    {
        anim = characPreview.GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.position;

        buttonCanvas = GetComponent<Canvas>();
        if (buttonCanvas == null)
        {
            buttonCanvas = gameObject.AddComponent<Canvas>();
            buttonCanvas.overrideSorting = true;
            buttonCanvas.sortingOrder = 0; // Valor padrão
        }
    }

    private void Update()
    {
        controller.ChooseButton(characID, originalPosition, moveSpeed, rectTransform, buttonCanvas);
    }

    public void OnSelect(BaseEventData eventData)
    {
        anim.SetInteger("type", characID);
        name.text = characName;
        buttonCanvas.sortingOrder = 1; // Coloca para frente
        controller.selected = characID;
        GameMain gm = GameObject.Find("GameManager").GetComponent<GameMain>();
        TwoPlayerSetup tps = GameObject.Find("GameManager").GetComponent<TwoPlayerSetup>();

        tps.player1Prefab = p1 ? gm.characters[characID].GetComponent<PlayerInput>() : gm.characters[characID].GetComponent<PlayerInput>();
        //                  condição    verdadeiro                                      falço
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonCanvas.sortingOrder = 0; // Coloca para trás

    }
}
