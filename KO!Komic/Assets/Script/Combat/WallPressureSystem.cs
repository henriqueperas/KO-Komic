using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPressureSystem : MonoBehaviour
{
    [Header("Configurações")]
    public int hitsToBreak = 5; // Número de golpes para quebrar a parede
    public float wallRange = 1f; // Distância para considerar "contra a parede"
    public LayerMask wallLayer; // Layer das paredes que podem quebrar

    [Header("Referências")]
    GameMain gm;
    [SerializeField] FightingCamera fCam;
    public GameObject[] players;
    public GameObject wallBreakVFX;
    public GameObject newBackground; // Novo cenário
    GameObject fc;

    private int[] hitCounts = new int[2];
    private bool hitDetect = true;
    [SerializeField] bool cutsceneActive = false;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameMain>();
        fc = GameObject.Find("MainCamera");
    }

    void Update()
    {
        if (gm.player1 == null || gm.player2 == null) return;

        players[0] = gm.player1;
        players[1] = gm.player2;

        if (cutsceneActive) return;

        for (int i = 0; i < players.Length; i++)
        {
            
            // Verifica se jogador está contra a parede
            if (IsAgainstWall(players[i].transform))
            {
                if (players[i].GetComponent<PlayerController>().inHit && hitDetect)
                {
                    hitCounts[i]++;
                    hitDetect = false;
                }

                if(!players[i].GetComponent<PlayerController>().inHit) hitDetect = true;

                Debug.Log($"Jogador {i + 1} pressionado contra a parede. Hits: {hitCounts[i]}");

                if (hitCounts[i] >= hitsToBreak)
                {
                    if (i == 0)
                    {
                        StartCoroutine(WallBreakCutscene(players[i], players[i+1]));
                    }
                    else
                    {
                        StartCoroutine(WallBreakCutscene(players[i], players[i-1]));
                    }
                    
                    hitCounts[i] = 0;
                    break;
                }
            }
            else
            {
                hitCounts[i] = 0; // Reseta se sair da parede
            }
        }
    }

    bool IsAgainstWall(Transform player)
    {
        // Verifica se há parede à esquerda ou direita do jogador
        bool hitLeft = Physics2D.Raycast(player.position, Vector2.left, wallRange, wallLayer);
        bool hitRight = Physics2D.Raycast(player.position, Vector2.right, wallRange, wallLayer);

        return hitLeft || hitRight;
    }

    IEnumerator WallBreakCutscene(GameObject defeatedPlayer, GameObject winedPlayer)
    {
        cutsceneActive = true;


        // 1. Detecta qual parede está sendo pressionada
        RaycastHit2D wallHit = GetWallHit(defeatedPlayer.transform);
        if (wallHit.collider == null) yield break;

        gm.isPausing = true;

        // 2. Desativa a parede quebrada
        wallHit.collider.enabled = false;

        // 3. Posiciona o VFX na parede correta
        Vector3 wallPosition = wallHit.point;
        Quaternion vfxRotation = Quaternion.identity;

        // Rotaciona o VFX conforme o lado da parede
        if (wallHit.normal.x > 0) // Parede à esquerda
        {
            vfxRotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            vfxRotation = Quaternion.Euler(0, -90, 0);
        }

        GameObject vfx = Instantiate(wallBreakVFX, new Vector3(wallPosition.x, wallPosition.y, wallPosition.z + 1), vfxRotation);
        fc.GetComponent<FightingCamera>().enabled = false;

        int mod;

        if (wallHit.normal.x > 0) // Parede à esquerda
        {
            mod = -1;
            //fCam.limitLeft -= 41.5f;
            //fCam.limitRight -= 41.5f;
        }
        else
        {
            mod = 1;
            //fCam.limitLeft += 41.5f;
            //fCam.limitRight += 41.5f;
        }

        //transição da camera
        fc.transform.position = new Vector3(fc.transform.position.x + (20* mod), fc.transform.position.y, -3);

        StartCoroutine(gm.SlowMotionRoutine());

        //posiciona o cenário
        newBackground.transform.position = new Vector3(newBackground.transform.position.x + (41.5f * mod), newBackground.transform.position.y, newBackground.transform.position.z + 3);

        // Ativa o novo cenário
        newBackground.SetActive(true);

        // 4. Faz o jogador cair no buraco
        Rigidbody2D rb = defeatedPlayer.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;

            defeatedPlayer.transform.position = new Vector3(defeatedPlayer.transform.position.x + (15 * mod), defeatedPlayer.transform.position.y + 2f, defeatedPlayer.transform.position.z);

            defeatedPlayer.GetComponent<Animator>().SetTrigger("Hit");

            defeatedPlayer.GetComponent<PlayerController>().moveInput.x = 10 * mod;

            yield return new WaitForSeconds(0.2f);

            defeatedPlayer.GetComponent<PlayerController>().moveInput.x = 0 * mod;

            winedPlayer.transform.position = new Vector3(winedPlayer.transform.position.x + (7 * mod), winedPlayer.transform.position.y + 2f, winedPlayer.transform.position.z);

            //rb.AddForce(Vector2.left * 10f, ForceMode2D.Impulse);
            


        }

        

        yield return new WaitForSeconds(0.5f);

        fc.GetComponent<FightingCamera>().enabled = true;

        wallHit.collider.enabled = true;

        
        gm.isPausing = false;


        // 6. Reseta estado após cutscene
        Destroy(vfx, 3f);
        cutsceneActive = false;
    }

    Vector3 GetWallPosition(Transform player)
    {
        // Detecta qual lado da parede está
        RaycastHit2D hitLeft = Physics2D.Raycast(player.position, Vector2.left, wallRange, wallLayer);
        if (hitLeft) return hitLeft.point;

        RaycastHit2D hitRight = Physics2D.Raycast(player.position, Vector2.right, wallRange, wallLayer);
        return hitRight.point;
    }

    IEnumerator SlideTransition(GameObject newBG)
    {
        // Desliza o cenário atual para fora e o novo para dentro
        float duration = 1f;
        float elapsed = 0;

        Vector3 oldPos = Camera.main.transform.position;
        Vector3 newPos = oldPos + Vector3.right * 10f; // Ajuste conforme necessário

        GameObject currentBG = GameObject.FindGameObjectWithTag("Background"); // Certifique-se de tagguear o cenário

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Move a câmera
            Camera.main.transform.position = Vector3.Lerp(oldPos, newPos, progress);

            // Move os cenários se necessário
            if (currentBG != null)
                currentBG.transform.position = Vector3.Lerp(oldPos, oldPos + Vector3.left * 10f, progress);

            yield return null;
        }

        // Ativa o novo cenário
        newBG.SetActive(true);
        if (currentBG != null)
            currentBG.SetActive(false);
    }

    RaycastHit2D GetWallHit(Transform player)
    {
        // Detecta parede à esquerda
        RaycastHit2D hitLeft = Physics2D.Raycast(
            player.position,
            Vector2.left,
            wallRange,
            wallLayer);

        if (hitLeft.collider != null)
        {
            Debug.DrawRay(player.position, Vector2.left * wallRange, Color.red, 2f);
            return hitLeft;
        }

        // Detecta parede à direita
        RaycastHit2D hitRight = Physics2D.Raycast(
            player.position,
            Vector2.right,
            wallRange,
            wallLayer);

        if (hitRight.collider != null)
        {
            Debug.DrawRay(player.position, Vector2.right * wallRange, Color.red, 2f);
            return hitRight;
        }

        return new RaycastHit2D();
    }
}
