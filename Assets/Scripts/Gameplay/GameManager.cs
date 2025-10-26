using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text scoreText, live, gameoverScore;
    public event Action OnGameOver;
    private int score;
    public GameObject GameOverPanel , SelectSpritePanel;
    [SerializeField] private int PlayerLives, LivesThreshhold = 0;

    [SerializeField] private List<Sprite> ArrowSprites;
    public Image ArrowSpriteRenderer;
    void Awake() => Instance = this;
    public void PauseGame()
    {
        SelectSpritePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void SelectArrowSprite(int index)
    {
        Time.timeScale = 1f;
        ObjectPool.Instance.SpriteChange(ArrowSprites[index]);
        ArrowSpriteRenderer.sprite = ArrowSprites[index];
        SelectSpritePanel.SetActive(false);
    }
    void Start()
    {
        score = 0;
        PlayerLives = LivesThreshhold;
        UpdateUI();
    }

    public void Hit()
    {
        score++;
        UpdateUI();
    }

    public void Miss()
    {
        PlayerLives--;
        if (PlayerLives <= 0)
            GameOver();
        //score = Mathf.Max(0, score - 1);
        UpdateUI();
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        Time.timeScale = 0f;
        GameOverPanel.SetActive(true);
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void UpdateUI()
    {
        if (scoreText)
        {
            scoreText.text = "Score: " + score;
        }
        if (gameoverScore)
        {
            gameoverScore.text = "Total Score : " + score;
        }
        if (live)
        {
            live.text = "Lives: " + PlayerLives;
        }
    }
}
