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

    private void Update()
    {
        if (!HasStateAuthority) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetComponentInChildren<SaveImage2>().TakeScreenshot();
        }
    }

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

        int points = Mathf.RoundToInt(similarity * 100);
        Score += points;

        Debug.Log($"Player scored {points} points! Total: {Score}");

        if (textoComparacao != null)
        {
            textoComparacao.text = $"Similarity: {similarity * 100:F2}%\nScore: {Score}";
        }
    }
}
