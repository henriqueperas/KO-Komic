using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SelectableButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Config")]
    public Vector2 selectedScale = new Vector2(1.2f, 1.2f); // Aumenta 20%
    public Vector2 selectedPosition;
    public float speed = 5f;
    public Sprite defaulSprite;
    public Sprite selectedSprite;
    public bool selected = false;
    [SerializeField] AudioClip SelectedSFX;

    private Vector2 originalScale;
    private Vector2 originalPosition;
    private RectTransform rectTransform;

    private Canvas buttonCanvas;

    public bool spriteImage;
    public Sprite otherSprite;
    public GameObject otherObject;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.position;

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
        SimpleSelection();

        if (otherObject != null && selected)
        {
            if (spriteImage)
            {
                otherObject.GetComponent<SpriteRenderer>().sprite = otherSprite;
            }
            else
            {
                otherObject.GetComponent<Image>().sprite = otherSprite;
            }

        }
        else
        {
            return;
        }
    }

    void SimpleSelection()
    {
        // Interpola suavemente o tamanho
        Vector2 targetScale = EventSystem.current.currentSelectedGameObject == gameObject ? (selectedScale + originalScale) : originalScale;
        rectTransform.localScale = Vector2.Lerp(rectTransform.localScale, targetScale, speed * Time.deltaTime);

        Vector2 targetPosition = EventSystem.current.currentSelectedGameObject == gameObject ? (selectedPosition + originalPosition) : originalPosition;
        rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, speed * Time.deltaTime);
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        // Efeito adicional (opcional)
        GetComponent<Image>().sprite = selectedSprite;
        buttonCanvas.sortingOrder = 2; // Coloca para frente
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        GetComponent<Image>().sprite = defaulSprite;
        buttonCanvas.sortingOrder = 0; // Volta para o padrão
    }
}
