using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem.UI;

public class MenuController : MonoBehaviour
{
    public InputActionReference navigateAction;
    public InputActionReference submitAction;
    //public InputActionReference cycleLettersAction; // Gatilhos/botões laterais
    public GameObject firstSelectedButton; // Primeiro botão selecionado

    void OnEnable()
    {
        // Ativa as ações
        //navigateAction.action.Enable();
        //submitAction.action.Enable();

        // Configura o primeiro botão selecionado
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        // Associa os eventos
        //submitAction.action.performed += OnSubmit;
    }

    void OnDisable()
    {
        submitAction.action.performed -= OnSubmit;
    }

    void OnSubmit(InputAction.CallbackContext ctx)
    {

        // Simula um clique no botão selecionado
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            ExecuteEvents.Execute(
                EventSystem.current.currentSelectedGameObject,
                new BaseEventData(EventSystem.current),
                ExecuteEvents.submitHandler
            );

        }
    }

    public void UI1Player()
    {
        var uiModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        //uiModule.actionsAsset. = navigateAction


        // Ativa as ações
        navigateAction.action.Enable();
        submitAction.action.Enable();

        // Configura o primeiro botão selecionado
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        // Associa os eventos
        submitAction.action.performed += OnSubmit;

    }

    public void NewButton(GameObject backButton)
    {
        print(backButton);
        EventSystem.current.SetSelectedGameObject(backButton);
    }
}
