using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public InputActionReference navigateAction;
    public InputActionReference submitAction;
    public GameObject firstSelectedButton; // Primeiro bot�o selecionado

    void OnEnable()
    {
        // Ativa as a��es
        navigateAction.action.Enable();
        submitAction.action.Enable();

        // Configura o primeiro bot�o selecionado
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        // Associa os eventos
        submitAction.action.performed += OnSubmit;
    }

    void OnDisable()
    {
        submitAction.action.performed -= OnSubmit;
    }

    void OnSubmit(InputAction.CallbackContext ctx)
    {
        // Simula um clique no bot�o selecionado
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            ExecuteEvents.Execute(
                EventSystem.current.currentSelectedGameObject,
                new BaseEventData(EventSystem.current),
                ExecuteEvents.submitHandler
            );
        }
    }

    public void NewButton(GameObject backButton)
    {
        EventSystem.current.SetSelectedGameObject(backButton);
    }
}
