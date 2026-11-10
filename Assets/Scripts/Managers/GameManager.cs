using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver,
        Victory
    }

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private JobStability jobStability;
    [SerializeField] private JobSwitcher jobSwitcher;
    [SerializeField] private OfficeMinigame officeMinigame;
    [SerializeField] private TaxiMinigame taxiMinigame;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;

    private float remainingTime;

    public bool IsPlaying =>
        CurrentState == GameState.Playing;

    public GameState CurrentState { get; private set; }

    private void Start()
    {
        CurrentState = GameState.MainMenu;
        remainingTime = gameDuration;

        UpdateTimerDisplay();
        ShowPanel(startPanel);
    }

    private void Update()
    {
        if (!IsPlaying)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerDisplay();
            WinGame();
            return;
        }

        UpdateTimerDisplay();
    }

    public void StartGame()
    {
        remainingTime = gameDuration;

        jobStability.ResetStability();
        jobSwitcher.ResetJobs();

        officeMinigame.ResetMinigame();

        if (taxiMinigame != null)
            taxiMinigame.ResetMinigame();

        CurrentState = GameState.Playing;

        UpdateTimerDisplay();
        ShowPanel(gameplayPanel);
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void LoseGame()
    {
        if (!IsPlaying)
            return;

        CurrentState = GameState.GameOver;
        ShowPanel(gameOverPanel);
    }

    public void WinGame()
    {
        if (!IsPlaying)
            return;

        CurrentState = GameState.Victory;
        ShowPanel(victoryPanel);
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        timerText.text =
            Mathf.CeilToInt(remainingTime).ToString("00");
    }

    private void ShowPanel(GameObject panelToShow)
    {
        startPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        panelToShow.SetActive(true);
    }
}
