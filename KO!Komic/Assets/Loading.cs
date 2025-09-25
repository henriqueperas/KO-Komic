using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [Header("Performance Config")]
    public int targetFPS = 60;
    public float stabilityThreshold = 0.9f; // 90% de estabilidade
    public float checkDuration = 3f; // Tempo de verificação
    public int minSamples = 30; // Mínimo de amostras antes de verificar
    [SerializeField] float coolDownCheck;

    [Header("UI References")]
    public GameObject loadingScreen;
    //public Slider progressSlider;
    public Image progressSlider;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI fpsText;

    [Header("Loading Config")]
    public bool preloadAssets = true;
    public List<GameObject> charactersLoad;
    public int progress = 0;
    public int progressMax;
    public GameObject newScreen;

    [Header("Main References")]
    public GameMain gm;

    private AsyncOperation loadingOperation;
    private Queue<float> fpsSamples = new Queue<float>();
    private float currentFPS;
    private bool isMonitoringFPS;
    private bool isStable;

    void Awake()
    {
        gm = GetComponent<GameMain>();

        /*
        // Garante que a tela de loading está ativa
        if (loadingScreen != null && !loadingScreen.activeSelf)
            loadingScreen.SetActive(true); */
    }

    public void LoadSceneWithSmartLoading()
    {
        StartCoroutine(SmartLoadScene());
    }

    public void LoadClear()
    {
        charactersLoad.Clear();
    }

    IEnumerator SmartLoadScene()
    {
        gm.isPausing = true;
        // Prepara ambiente de loading
        ResetMonitoring();
        isMonitoringFPS = true;

        if(gm.player1 == null && gm.player2 == null)
        {
            for(int i = 0; i <= gm.characters.Length; i++)
            {
                charactersLoad.Add(gm.characters[i]);
            } 
        }
        else
        {
            charactersLoad.Add(gm.player1);
            charactersLoad.Add(gm.player2);
        }

        progressMax = (charactersLoad.Count * 16) * 4;

        charactersLoad[0].GetComponent<Animator>().runtimeAnimatorController = charactersLoad[0].GetComponent<PlayerMain>().animLoading;
        charactersLoad[1].GetComponent<Animator>().runtimeAnimatorController = charactersLoad[1].GetComponent<PlayerMain>().animLoading;

        for (int i = 0; i <= charactersLoad.Count - 1; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                charactersLoad[i].GetComponent<Animator>().SetFloat("Index", (j + 1));
                yield return new WaitForSeconds(coolDownCheck);

                if (currentFPS < 50)
                {
                    j--;
                }
                else
                {
                    progress += j;
                }

                MonitorFPS();
                UpdateProgressUI();
            }
            UpdateProgressUI();
        }

        charactersLoad[0].GetComponent<Animator>().runtimeAnimatorController = charactersLoad[0].GetComponent<PlayerMain>().animFight;
        charactersLoad[1].GetComponent<Animator>().runtimeAnimatorController = charactersLoad[1].GetComponent<PlayerMain>().animFight;

        ScreenDesteny(newScreen);
        gm.PrepareCharacter();

        isMonitoringFPS = !isMonitoringFPS;

        if (gm.training)
        {
            gm.trainingScreen.SetActive(true);
        }
        
    }

    void Update()
    {
        if (isMonitoringFPS)
        {
            // Calcula FPS atual
            currentFPS = 1f / Time.unscaledDeltaTime;

            // Atualiza UI se existir
            if (fpsText != null)
                fpsText.text = $"FPS: {currentFPS:F1} / {targetFPS}";

            // Coleta amostras
            if (fpsSamples.Count >= minSamples)
                fpsSamples.Dequeue();

            fpsSamples.Enqueue(currentFPS);
        }
        else
        {
            fpsText.text = null;
        }
    }

    public void SetNewScreen(GameObject screen)
    {
        newScreen = screen;
    }

    public void ScreenDesteny(GameObject screen)
    {
        GetComponent<UIManager>().ChangeScreen(screen);
    }

    void MonitorFPS()
    {
        if (fpsSamples.Count < minSamples)
            return;

        // Calcula estabilidade
        float stabilityScore = CalculateStability();

        // Verifica se atingiu estabilidade
        if (stabilityScore >= stabilityThreshold)
        {
            isStable = true;
            Debug.Log($"FPS estabilizado! Score: {stabilityScore:P0}");
        }
    }

    float CalculateStability()
    {
        int stableSamples = 0;
        float totalSamples = fpsSamples.Count;

        foreach (float fps in fpsSamples)
        {
            // Considera estável se estiver dentro de 10% do target
            if (fps >= targetFPS * 0.9f && fps <= targetFPS * 1.1f)
            {
                stableSamples++;
            }
        }

        return stableSamples / totalSamples;
    }

    void UpdateProgressUI()
    {
        if (progressSlider != null)
        {
            // Progresso vai até 0.9 (90%) porque allowSceneActivation = false
            float progressBar = (float)progress / (float)progressMax;
            progressSlider.fillAmount = progressBar;
        }

        if (progressText != null)
        {
            float percentage = Mathf.Clamp01(progress / 0.9f) * 100f;
            progressText.text = $"{percentage:F0}%";

            if (isStable)
                progressText.text += " - Estável ✓";
        }
    }

    void ResetMonitoring()
    {
        fpsSamples.Clear();
        isMonitoringFPS = false;
        isStable = false;
    }

    // Método para forçar liberação (caso necessário)
    public void ForceSceneActivation()
    {
        if (loadingOperation != null)
        {
            loadingOperation.allowSceneActivation = true;
            isStable = true;
        }
    }

    // Método público para verificar status
    public bool IsFPSStable()
    {
        return isStable;
    }

    public float GetCurrentStability()
    {
        return CalculateStability();
    }
}
