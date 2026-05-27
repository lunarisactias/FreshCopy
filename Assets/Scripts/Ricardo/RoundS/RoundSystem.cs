using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static Fusion.NetworkBehaviour;

public class RoundSystem : NetworkBehaviour
{
    public enum GameState
    {
        Preparing,
        Memorizing,
        Drawing,
        GameOver
    }

    [Header("CHOSEN IMAGES")]
    [SerializeField] private Texture2D[] imgs;

    [Header("DURATIONS (Seconds)")]
    [SerializeField] private float preparingDuration = 5f;
    [SerializeField] private float memorizingDuration = 10f;
    [SerializeField] private float drawingDuration = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject scoreboardScreen;
    [SerializeField] TextMeshProUGUI scoreboardText;
 
    [Header("SCRIPTS")]
    [SerializeField] private SentisComparer imgComparer;
    [SerializeField] private Reference refer;

    [Networked] public TickTimer RoundTimer { get; set; }
    [Networked] public int ChosenImageIndex { get; set; }
    [Networked] public GameState CurrentState { get; set; }

    private ChangeDetector changeDetector;
    private float scoreboardUpdateTimer = 0f;

    public override void Spawned()
    {
        if (imgComparer == null) imgComparer = FindAnyObjectByType<SentisComparer>();
        if (refer == null) refer = FindAnyObjectByType<Reference>();
        if (scoreboardScreen == null) scoreboardScreen.SetActive(false);

        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (HasStateAuthority)
        {
            ChosenImageIndex = -1;
            CurrentState = GameState.Preparing;
            RoundTimer = TickTimer.CreateFromSeconds(Runner, preparingDuration);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (RoundTimer.Expired(Runner))
        {
            AdvanceGameState();
        }
    }

    private void AdvanceGameState()
    {
        switch (CurrentState)
        {
            case GameState.Preparing:
                ChosenImageIndex = Random.Range(0, imgs.Length);
                CurrentState = GameState.Memorizing;
                RoundTimer = TickTimer.CreateFromSeconds(Runner, memorizingDuration);
                break;
            case GameState.Memorizing:
                CurrentState = GameState.Drawing;
                RoundTimer = TickTimer.CreateFromSeconds(Runner, drawingDuration);
                break;
            case GameState.Drawing:
                CurrentState = GameState.GameOver;
                scoreboardScreen.SetActive(true);
                break;
            case GameState.GameOver:
                break;
        }
    }

    public override void Render()
    {
        if (RoundTimer.IsRunning)
        {
            int secInt = Mathf.CeilToInt(RoundTimer.RemainingTime(Runner) ?? 0);
            timerText.text = secInt.ToString();
        }

        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(CurrentState):
                    UpdateScreenByState(CurrentState);
                    break;
            }
        }

        if (CurrentState == GameState.GameOver)
        {
            scoreboardUpdateTimer += Time.deltaTime;
            if (scoreboardUpdateTimer >= 0.5f)
            {
                scoreboardUpdateTimer = 0f;
                DisplayNamesOnScoreboard();
            }
        }
    }

    private void UpdateScreenByState(GameState state)
    {
        switch (state)
        {
            case GameState.Memorizing:
                if (ChosenImageIndex >= 0)
                {
                    Texture2D chosenImage = imgs[ChosenImageIndex];

                    refer.gameObject.SetActive(true);
                    refer.ShowImageReference(chosenImage);

                    Player localPlayer = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault(p => p.HasStateAuthority);
                    if (localPlayer != null)
                    {
                        SaveImage2 screenshot = localPlayer.GetComponentInChildren<SaveImage2>();
                        if (screenshot != null)
                        {
                            screenshot.TakeReferenceScreenshot();
                        }
                    }
                }
                break;

            case GameState.Drawing:
                refer.HideImageReference();
                break;

            case GameState.GameOver:
                EndMatchAndGenerateScoreboard();
                break;
        }
    }

    private void EndMatchAndGenerateScoreboard()
    {
        Player localPlayer = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault(p => p.HasStateAuthority);
        if (localPlayer != null && SentisComparer.Instance != null)
        {
            localPlayer.IsEvaluated = false;

            localPlayer.SetDrawing(null);

            SaveImage2 screenshot = localPlayer.GetComponentInChildren<SaveImage2>();
            if (screenshot != null)
            {
                screenshot.TakeScreenshot();
            }

            SentisComparer.Instance.EvaluateLocalPlayer(localPlayer);
        }

        if (scoreboardScreen != null) scoreboardScreen.SetActive(true);

        DisplayNamesOnScoreboard();
    }

    private void DisplayNamesOnScoreboard()
    {
        if (scoreboardText == null) return;

        var allPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None)
                            .OrderByDescending(p => p.Score)
                            .ToList();

        scoreboardText.text = "<b>Notas Finais</b>\n\n";

        int position = 1;
        foreach (var p in allPlayers)
        {
            string pointsDisplay = p.IsEvaluated ? $"{p.Score} Pontos" : "Processando...";
            scoreboardText.text += $"{position}º Lugar: {p.NomeJogador} - {p.Score} Pontos\n";
            position++;
        }
    }
}
