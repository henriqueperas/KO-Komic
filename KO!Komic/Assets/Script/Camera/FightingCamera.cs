using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingCamera : MonoBehaviour
{
    [Header("Jogadores")]
    [SerializeField] GameMain gm;
    public Transform player1;
    public Transform player2;

    [Header("Configurações de Zoom")]
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomSpeed = 2f;
    public float minDistance = 4f;
    public float maxDistance = 15f;

    [Header("Configurações de Posição")]
    public float yOffset = 1f;
    public float followSmoothness = 5f;

    [Header("Efeito de Hit")]
    public float hitZoomAmount = 0.5f;
    public float hitZoomDuration = 0.2f;
    public float hitShakeIntensity = 0.1f;
    public float hitShakeDuration = 0.1f;

    Camera cam;
    float targetZoom;
    Vector3 targetPosition;
    float originalZoom;
    bool isHitEffectActive;
    float hitEffectTimer;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalZoom = cam.orthographicSize;
        targetZoom = originalZoom;
    }

    void LateUpdate()
    {
        player1 = gm.player1.transform;
        player2 = gm.player2.transform;

        if (player1 == null || player2 == null) return;

        // Calcula posição média horizontal entre os jogadores (mas mantém a posição Y fixa)
        float averageX = (player1.position.x + player2.position.x) / 2f;
        targetPosition = new Vector3(averageX, 0.2f, transform.position.z);

        // Calcula distância entre os jogadores
        float distance = Mathf.Abs(player1.position.x - player2.position.x);

        // Calcula o zoom baseado na distância (com clamp nos valores)
        float zoomRatio = Mathf.InverseLerp(minDistance, maxDistance, distance);
        targetZoom = Mathf.Lerp(minZoom, maxZoom, zoomRatio);

        // Aplica zoom suavemente
        // cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        // Move a câmera suavemente (apenas no eixo X)
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSmoothness);

        // Trata efeito de hit
        if (isHitEffectActive)
        {
            HandleHitEffect();
        }
    }

    // Método para ser chamado quando um golpe acertar
    public void TriggerHitEffect()
    {
        isHitEffectActive = true;
        hitEffectTimer = 0f;
    }

    void HandleHitEffect()
    {
        hitEffectTimer += Time.deltaTime;

        if (hitEffectTimer < hitZoomDuration)
        {
            // Zoom rápido
            float zoomProgress = hitEffectTimer / hitZoomDuration;
            float currentZoomEffect = Mathf.Lerp(hitZoomAmount, 0f, zoomProgress);
            cam.orthographicSize = targetZoom - currentZoomEffect;

            // Pequeno shake
            if (hitEffectTimer < hitShakeDuration)
            {
                float shakeX = Random.Range(-hitShakeIntensity, hitShakeIntensity);
                float shakeY = Random.Range(-hitShakeIntensity, hitShakeIntensity);
                transform.position += new Vector3(shakeX, shakeY, 0);
            }
        }
        else
        {
            isHitEffectActive = false;
        }
    }

    // Desenha gizmos para visualização no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (player1 != null && player2 != null)
        {
            Gizmos.DrawLine(player1.position, player2.position);
            Vector3 midpoint = new Vector3((player1.position.x + player2.position.x) / 2,
                                         (player1.position.y + player2.position.y) / 2, 0);
            Gizmos.DrawWireSphere(midpoint, 0.5f);
        }
    }
}
