using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject namePanel;
    public TMP_InputField nameInput;
    public TMP_Text welcomeText;
    public GameObject enterCorrectName;
    private const string PlayerNameKey = "PlayerName";

    private void Start()
    {
        // Check if player name is already saved
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            string savedName = PlayerPrefs.GetString(PlayerNameKey);
            namePanel.SetActive(false);
            ShowWelcome(savedName);
        }
        else
        {
            namePanel.SetActive(true);
        }
    }

    public void OnConfirmName()
    {
        string playerName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Name cannot be empty!");
            enterCorrectName.transform.DOLocalMoveY(526, 0);
            enterCorrectName.transform.DOLocalMoveY(950, 0.5f).SetEase(Ease.OutBounce);
            enterCorrectName.transform.DOLocalMoveY(526, 0.5f).SetDelay(1);
            return;
        }

        // Save name
        PlayerPrefs.SetString(PlayerNameKey, playerName);
        PlayerPrefs.Save();

        // Hide panel and show welcome text
        namePanel.SetActive(false);
        ShowWelcome(playerName);
    }

    private void ShowWelcome(string name)
    {
        if (welcomeText != null)
        {
            welcomeText.text = $"Welcome, {name}!";
        }
    }
}
