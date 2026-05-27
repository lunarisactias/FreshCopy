using System.IO;
using TMPro;
using UnityEngine;
using Fusion;
using System.Configuration;

public class Player : NetworkBehaviour
{
    public Texture2D playerDrawing;
    public TextMeshProUGUI textoComparacao;

    [Networked] public int Score { get; set; }
    [Networked] public NetworkString<_32> NomeJogador { get; set; }
    [Networked] public NetworkBool IsEvaluated { get; set; }

    private ChangeDetector changeDetector;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

   

    public override void Spawned()
    {
        DontDestroyOnLoad(gameObject);

        if (HasStateAuthority)
        {
            NomeJogador = PlayerPrefs.GetString("NomeJogadorLocal", "Visitante");
        }

        if (MultiplayerController.Instance != null)
        {
            MultiplayerController.Instance.RegistrarJogadorNaUI(this);
        }

        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch (change)
            { 
            case nameof(Score):
                if (textoComparacao != null)
                {
                    textoComparacao.text = Score.ToString();
                }
                break;
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (MultiplayerController.Instance != null)
        {
            MultiplayerController.Instance.RemoverJogadorDaUI(this);
        }
    }

    public void SetDrawing(Texture2D drawing)
    {
        if (playerDrawing != null)
        {
            Destroy(playerDrawing);
        }

        playerDrawing = drawing;
    }

    public Texture2D GetDrawing()
    {
        return playerDrawing;
    }

    public void AddScore(float similarity)
    {
        if (!HasStateAuthority) return;

        int points = Mathf.RoundToInt(Mathf.Max(0, similarity) * 100);
        Score += points;

        IsEvaluated = true;

        Debug.Log($"Player scored {points} points! Total: {Score}");

        if (textoComparacao != null)
        {
            textoComparacao.text = $"Similarity: {similarity * 100:F2}%\nScore: {Score}";
        }
    }
}
