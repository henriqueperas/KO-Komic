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

    public bool ready;

    public float moveSpeed = 1f;

    [SerializeField] GameObject characPreviewObj;

    [SerializeField] Sprite characPreviewImage;
    Animator anim;
    [SerializeField] TextMeshProUGUI name;

    private RectTransform rectTransform;

    [SerializeField] CharacterSelectionController controller;

    private Vector2 originalPosition;

    private void Start()
    {
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
        characPreviewObj.SetActive(true);
        characPreviewObj.GetComponent<Image>().sprite = characPreviewImage;
        name.text = characName;
        buttonCanvas.sortingOrder = 1; // Coloca para frente
        controller.selected = characID;
        GameMain gm = GameObject.Find("GameManager").GetComponent<GameMain>();
        TwoPlayerSetup tps = GameObject.Find("GameManager").GetComponent<TwoPlayerSetup>();

        if (p1)
        {
            gm.player1 = gm.characters[characID];
        }
        else
        {
            gm.player1 = gm.characters[characID];
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonCanvas.sortingOrder = 0; // Coloca para trás
        characPreviewObj.SetActive(false);

    }

    public void PlayerReady()
    {
        GameMain gm = GameObject.Find("GameManager").GetComponent<GameMain>();
        if (!ready)
        {
            gm.playersReady += 1;
            ready = true;
        }
        else
        {
            gm.playersReady -= 1;
            ready = false;
        }
    }
}
