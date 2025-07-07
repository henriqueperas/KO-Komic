using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Config")]
    public Vector2 selectedScale = new Vector2(1.2f, 1.2f); // Aumenta 20%
    public float scaleSpeed = 5f;

    private Vector2 originalScale;
    private RectTransform rectTransform;

    private Canvas buttonCanvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        buttonCanvas = GetComponent<Canvas>();
        if (buttonCanvas == null)
        {
            buttonCanvas = gameObject.AddComponent<Canvas>();
            buttonCanvas.overrideSorting = true;
            buttonCanvas.sortingOrder = 0; // Valor padrão
        }
    }

    void Update()
    {
        // Interpola suavemente o tamanho
        Vector2 targetScale = EventSystem.current.currentSelectedGameObject == gameObject ? selectedScale : originalScale;
        rectTransform.localScale = Vector2.Lerp(rectTransform.localScale, targetScale, scaleSpeed * Time.deltaTime);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Efeito adicional (opcional)
        GetComponent<Image>().color = Color.yellow;
        buttonCanvas.sortingOrder = 1; // Coloca para frente
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
        buttonCanvas.sortingOrder = 0; // Volta para o padrão
    }
}
