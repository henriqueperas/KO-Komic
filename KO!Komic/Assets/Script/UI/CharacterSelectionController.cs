using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;

public class CharacterSelectionController : MonoBehaviour
{
    public Vector2[] positionsButton;

    public Vector2 selectedButton;

    public int selected;

    public void ChooseButton(int ID, Vector2 originalPosition, float moveSpeed, RectTransform rectTransform, Canvas buttonCanvas)
    {
        Vector2 targetPosition;

        if (selected == ID)
        {
            targetPosition = EventSystem.current.currentSelectedGameObject == gameObject ? positionsButton[1] : originalPosition;
            rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            buttonCanvas.sortingOrder = 6; // Coloca para frente
        }
        else if (selected < ID)
        {
            targetPosition = gameObject ? positionsButton[2] : originalPosition;
            rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            buttonCanvas.sortingOrder = -(selected % ID) + 5; // Coloca para trás
        }
        else if (selected > ID)
        {
            targetPosition = gameObject ? positionsButton[0] : originalPosition;
            rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            buttonCanvas.sortingOrder = selected % ID + 5; // Coloca para trás
        }
    }

}
