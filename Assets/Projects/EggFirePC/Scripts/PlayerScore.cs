using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] Canvas ownCanvas;
    [SerializeField] int enemiesList;

    private int score;
    private TextMeshProUGUI scoreTxt;
    private int totalEnemies;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalEnemies=enemiesList;
        score=0;
        scoreTxt=ownCanvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
        scoreTxt.text=$"{score}/{totalEnemies}";
    }

    public void AddScore()
    {
        score++;
        scoreTxt.text=$"{score}/{totalEnemies}";
    }
}
