using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] public TMP_Text[] entryTexts;
    [SerializeField] private LeaderboardSystem leaderboard;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        List<HighscoreData> highscores = leaderboard.GetHighscores();

        for (int i = 0; i < entryTexts.Length; i++)
        {
            if (i < highscores.Count)
            {
                HighscoreData entry = highscores[i];
                entryTexts[i].text = $"{i + 1}. {entry.playerName} - {entry.score}";
            }
            else
            {
                entryTexts[i].text = $"{i + 1}. ---";
            }
        }
    }
}
