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
        Vector2 targetPosition;

        if (controller.selected == characID)
        {
            targetPosition = EventSystem.current.currentSelectedGameObject == gameObject ? controller.positionsButton[1] : originalPosition;
            rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            buttonCanvas.sortingOrder = 6; // Coloca para frente
        }
        else if (controller.selected < characID)
        {
            targetPosition = gameObject ? controller.positionsButton[2] : originalPosition;
            rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            buttonCanvas.sortingOrder = -(controller.selected % characID) + 5; // Coloca para trás
        }
        else if (controller.selected > characID) 
        {
            targetPosition = gameObject ? controller.positionsButton[0] : originalPosition;
            rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            buttonCanvas.sortingOrder = controller.selected % characID + 5; // Coloca para trás
        }

    }

    public void OnSelect(BaseEventData eventData)
    {
        anim.SetInteger("type", characID);
        name.text = characName;
        buttonCanvas.sortingOrder = 1; // Coloca para frente
        controller.selected = characID;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonCanvas.sortingOrder = 0; // Coloca para trás

    }
}
