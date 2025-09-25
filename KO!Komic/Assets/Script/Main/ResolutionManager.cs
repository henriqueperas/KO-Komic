using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    [System.Serializable]
    public class ResolutionOption
    {
        public int width;
        public int height;
        public string label; // Ex: "1920x1080 (Full HD)"
    }

    [Header("Referências UI")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TextMeshProUGUI confirmationText;

    [Header("Configurações")]
    public List<ResolutionOption> availableResolutions = new List<ResolutionOption>
    {
        new ResolutionOption { width = 3840, height = 2160, label = "4K Ultra HD" },
        new ResolutionOption { width = 2560, height = 1440, label = "1440p (QHD)" },
        new ResolutionOption { width = 1920, height = 1080, label = "1080p (Full HD)" },
        new ResolutionOption { width = 1366, height = 768, label = "1366x768" },
        new ResolutionOption { width = 1280, height = 720, label = "720p (HD)" }
    };

    public float confirmationDisplayTime = 3f;

    private int currentResolutionIndex;
    private bool currentFullscreenState;
    private Resolution currentResolution;
    private Coroutine confirmationCoroutine;

    void Start()
    {
        InitializeSettings();
        SetupDropdown();
        SetupEventListeners();
        LoadSavedSettings();
    }

    /// <summary>
    /// Inicializa as configurações atuais
    /// </summary>
    void InitializeSettings()
    {
        currentResolution = Screen.currentResolution;
        currentFullscreenState = Screen.fullScreen;

        // Encontra a resolução atual na lista
        currentResolutionIndex = FindCurrentResolutionIndex();
    }

    /// <summary>
    /// Configura o dropdown com as resoluções disponíveis
    /// </summary>
    void SetupDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var res in availableResolutions)
        {
            options.Add(res.label);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Configura os listeners de eventos
    /// </summary>
    void SetupEventListeners()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
            fullscreenToggle.isOn = currentFullscreenState;
        }
    }

    /// <summary>
    /// Encontra o índice da resolução atual na lista
    /// </summary>
    int FindCurrentResolutionIndex()
    {
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (Screen.width == availableResolutions[i].width &&
                Screen.height == availableResolutions[i].height)
            {
                return i;
            }
        }
        return 0; // Default para primeira resolução
    }

    /// <summary>
    /// Callback quando a resolução é alterada no dropdown
    /// </summary>
    void OnResolutionChanged(int index)
    {
        currentResolutionIndex = index;
        Debug.Log($"Resolução selecionada: {availableResolutions[index].label}");
    }

    /// <summary>
    /// Callback quando o toggle de fullscreen é alterado
    /// </summary>
    void OnFullscreenToggle(bool isFullscreen)
    {
        currentFullscreenState = isFullscreen;
        Debug.Log($"Fullscreen: {(isFullscreen ? "Ativado" : "Desativado")}");
    }

    /// <summary>
    /// Aplica as configurações selecionadas
    /// </summary>
    public void ApplySettings()
    {
        if (currentResolutionIndex < 0 || currentResolutionIndex >= availableResolutions.Count)
            return;

        ResolutionOption selectedRes = availableResolutions[currentResolutionIndex];

        // Aplica resolução e fullscreen
        Screen.SetResolution(selectedRes.width, selectedRes.height, currentFullscreenState);

        Debug.Log($"Resolução aplicada: {selectedRes.width}x{selectedRes.height}, Fullscreen: {currentFullscreenState}");

        // Salva as configurações
        SaveSettings();

        // Mostra confirmação
        ShowConfirmation("Configurações aplicadas com sucesso!");

        // Atualiza referência atual
        currentResolution = Screen.currentResolution;
    }

    /// <summary>
    /// Mostra mensagem de confirmação
    /// </summary>
    void ShowConfirmation(string message)
    {
        if (confirmationText != null)
        {
            confirmationText.text = message;
            confirmationText.gameObject.SetActive(true);

            if (confirmationCoroutine != null)
                StopCoroutine(confirmationCoroutine);

            confirmationCoroutine = StartCoroutine(HideConfirmationAfterDelay());
        }
    }

    IEnumerator HideConfirmationAfterDelay()
    {
        yield return new WaitForSeconds(confirmationDisplayTime);
        confirmationText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Salva as configurações no PlayerPrefs
    /// </summary>
    void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        PlayerPrefs.SetInt("Fullscreen", currentFullscreenState ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carrega as configurações salvas
    /// </summary>
    void LoadSavedSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            if (resolutionDropdown != null)
            {
                resolutionDropdown.value = currentResolutionIndex;
            }
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            currentFullscreenState = PlayerPrefs.GetInt("Fullscreen") == 1;
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = currentFullscreenState;
            }
        }

        // Aplica as configurações salvas
        if (PlayerPrefs.HasKey("ResolutionIndex") || PlayerPrefs.HasKey("Fullscreen"))
        {
            ApplySettings();
        }
    }

    /// <summary>
    /// Alterna entre fullscreen e janela
    /// </summary>
    public void ToggleFullscreen()
    {
        currentFullscreenState = !currentFullscreenState;

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = currentFullscreenState;
        }

        Screen.fullScreen = currentFullscreenState;
        SaveSettings();

        ShowConfirmation($"Modo {((currentFullscreenState) ? "Fullscreen" : "Janela")} ativado");
    }

    /// <summary>
    /// Aplica a resolução nativa do monitor
    /// </summary>
    public void SetNativeResolution()
    {
        Resolution nativeRes = GetNativeResolution();
        int nativeIndex = FindNativeResolutionIndex(nativeRes);

        if (nativeIndex >= 0)
        {
            resolutionDropdown.value = nativeIndex;
            currentResolutionIndex = nativeIndex;
            ApplySettings();
        }
    }

    /// <summary>
    /// Obtém a resolução nativa do monitor
    /// </summary>
    Resolution GetNativeResolution()
    {
        Resolution[] resolutions = Screen.resolutions;
        return resolutions[resolutions.Length - 1]; // Maior resolução
    }

    /// <summary>
    /// Encontra o índice da resolução nativa na lista
    /// </summary>
    int FindNativeResolutionIndex(Resolution nativeRes)
    {
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (nativeRes.width == availableResolutions[i].width &&
                nativeRes.height == availableResolutions[i].height)
            {
                return i;
            }
        }
        return -1;
    }

    // Método para debug
    [ContextMenu("Debug Current Resolution")]
    void DebugCurrentResolution()
    {
        Debug.Log($"Current: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
        Debug.Log($"Selected: {availableResolutions[currentResolutionIndex].width}x{availableResolutions[currentResolutionIndex].height}");
    }
}
