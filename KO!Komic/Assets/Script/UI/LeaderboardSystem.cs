using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardSystem : MonoBehaviour
{
    private List<HighscoreData> highscores = new List<HighscoreData>();
    private const int MaxEntries = 6; // Máximo de entradas salvas

    [SerializeField] LeaderboardUI lUI;

    void Start()
    {
        LoadHighscores();
    }

    // Adiciona nova pontuação e salva
    public void AddHighscore(string name, int score)
    {
        highscores.Add(new HighscoreData(name, score));

        // Ordena do maior para o menor
        highscores.Sort((a, b) => b.score.CompareTo(a.score));

        // Mantém só os top 10
        if (highscores.Count > MaxEntries)
        {
            highscores.RemoveRange(MaxEntries, highscores.Count - MaxEntries);
        }

        SaveHighscores();
        lUI.UpdateUI();
    }

    // Salva a lista completa
    private void SaveHighscores()
    {
        string json = JsonUtility.ToJson(new HighscoreWrapper(highscores));
        PlayerPrefs.SetString("Leaderboard", json);
        PlayerPrefs.Save();
    }
    public void ResetLeaderboard()
    {
        PlayerPrefs.DeleteKey("Leaderboard");
        highscores.Clear();
    }

    // Carrega a lista salva
    private void LoadHighscores()
    {
        if (PlayerPrefs.HasKey("Leaderboard"))
        {
            string json = PlayerPrefs.GetString("Leaderboard");
            HighscoreWrapper wrapper = JsonUtility.FromJson<HighscoreWrapper>(json);
            highscores = wrapper.highscores;
        }
    }

    // Retorna a lista para exibição
    public List<HighscoreData> GetHighscores()
    {
        return highscores;
    }

    // Classe auxiliar para serialização
    [Serializable]
    private class HighscoreWrapper
    {
        public List<HighscoreData> highscores;
        public HighscoreWrapper(List<HighscoreData> highscores) => this.highscores = highscores;
    }
}

[System.Serializable]
public class HighscoreData
{
    public string playerName;
    public int score;

    public HighscoreData(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }
}

