using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerController : MonoBehaviour, INetworkRunnerCallbacks
{
    public static MultiplayerController Instance;

    [Header("Configurações do Menu")]
    public InputField nomeSala;
    public InputField nomeDoJogadorLocal;
    public InputField senhaDaSala;
    public Text erro;
    public Button BotaoEntrarSala;
    public Button BotaoAtualizarSalas;
    public GameObject playerPrefab;

    [Header("Telas (Canvas)")]
    public Canvas TelaEntrarSala;
    public Canvas TelaAguardandoSala;

    [Header("Elementos da Sala de Espera")]
    public Text listaJogadoresNaSala;
    public GameObject botaoComecarJogo;

    [Header("Lobby de Salas")]
    public List<SessionInfo> salasDisponiveis = new List<SessionInfo>();
    public Text listaLobby;

    private NetworkRunner runner;

    private List<Player> jogadoresConectados = new List<Player>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (TelaAguardandoSala != null)
            TelaAguardandoSala.gameObject.SetActive(false);

        ListarSalas();
    }
    public async void EntrarSala()
    {
        if (BotaoEntrarSala != null)
            BotaoEntrarSala.interactable = false;

        if (erro != null)
            erro.text = "Conectando...";

        if (string.IsNullOrEmpty(nomeSala.text))
        {
            Debug.LogError("O nome da sala não pode ser vazio!");
            erro.text = "O nome da sala não pode ser vazio!";
            if (BotaoEntrarSala != null)
                BotaoEntrarSala.interactable = true;
            return;
        }

        string nome = nomeSala.text;
        string senha = senhaDaSala != null ? senhaDaSala.text : "";

        SessionInfo salaEncontrada = null;
        foreach (var session in salasDisponiveis)
        {
            if (session.Name == nome)
            {
                salaEncontrada = session;
                break;
            }
        }

        if (salaEncontrada != null)
        {
            if (salaEncontrada.Properties != null && salaEncontrada.Properties.TryGetValue("Senha", out SessionProperty senhaSalva))
            {
                if ((string)senhaSalva != senha)
                {
                    Debug.LogError("Senha incorreta para a sala!");
                    erro.text = "Senha incorreta para a sala!";
                    if (BotaoEntrarSala != null)
                        BotaoEntrarSala.interactable = true;
                    return;
                }
            }
        }

        var customProps = new Dictionary<string, SessionProperty>();
        customProps.Add("Senha", senha);

        string nomeJogador = string.IsNullOrEmpty(nomeDoJogadorLocal?.text) ? "Jogador " + UnityEngine.Random.Range(1000, 9999) : nomeDoJogadorLocal.text;
        PlayerPrefs.SetString("NomeJogadorLocal", nomeJogador);

        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
        }

        var resultado = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = nomeSala.text,
            PlayerCount = 8,
            SessionProperties = customProps,
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (!resultado.Ok)
        {
            Debug.LogError("Falha ao entrar na sala: " + resultado.ShutdownReason);
            if (erro != null)
                erro.text = "Falha ao entrar na sala: " + resultado.ShutdownReason;
            if (BotaoEntrarSala != null)
                BotaoEntrarSala.interactable = true;
            return;
        }

        if (erro != null)
            erro.text = "";

        TelaEntrarSala.gameObject.SetActive(false);
        TelaAguardandoSala.gameObject.SetActive(true);

        if (botaoComecarJogo != null)
        {
            botaoComecarJogo.SetActive(runner.IsSharedModeMasterClient);
        }
    }

    public void IniciarJogo()
    {
        if (runner != null && runner.IsSharedModeMasterClient)
        {
            runner.SessionInfo.IsOpen = false;
            runner.SessionInfo.IsVisible = false;
            runner.LoadScene(SceneRef.FromIndex(1));
        }
    }

    public async void ListarSalas()
    {
        if (BotaoAtualizarSalas != null)
            BotaoAtualizarSalas.interactable = false;

        if (listaLobby != null)
            listaLobby.text = "Carregando salas...";

        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
        }
        var resultado = await runner.JoinSessionLobby(SessionLobby.Shared);

        if (!resultado.Ok)
        {
            Debug.LogError("Falha ao listar salas: " + resultado.ShutdownReason);
            if (listaLobby != null)
                listaLobby.text = "Falha ao listar salas: " + resultado.ShutdownReason;
            if (BotaoAtualizarSalas != null)
                BotaoAtualizarSalas.interactable = true;
            return;
        }

        await System.Threading.Tasks.Task.Delay(1000);

        if (BotaoAtualizarSalas != null)
            BotaoAtualizarSalas.interactable = true;
    }

    public void RegistrarJogadorNaUI(Player novoJogador)
    {
        if (!jogadoresConectados.Contains(novoJogador))
        {
            jogadoresConectados.Add(novoJogador);
            AtualizarListaDeEspera();
        }
    }

    public void RemoverJogadorDaUI(Player jogadorSaindo)
    {
        if (jogadoresConectados.Contains(jogadorSaindo))
        {
            jogadoresConectados.Remove(jogadorSaindo);
            AtualizarListaDeEspera();
        }
    }

    public void AtualizarListaDeEspera()
    {
        if (listaJogadoresNaSala == null) return;

        listaJogadoresNaSala.text = "Jogadores na Sala:\n\n";
        foreach (var player in jogadoresConectados)
        {
            listaJogadoresNaSala.text += $"- {player.NomeJogador}\n";
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) {}

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        TelaAguardandoSala.gameObject.SetActive(false);
        TelaEntrarSala.gameObject.SetActive(true);

        if (erro != null)
        {
            erro.text = "Desconectado: " + reason.ToString();
        }
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

    public void OnInput(NetworkRunner runner, NetworkInput input) {}

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.LocalPlayer != PlayerRef.None && runner.GetPlayerObject(runner.LocalPlayer) == null)
        {
            var objetoDaRede = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer);
            runner.SetPlayerObject(runner.LocalPlayer, objetoDaRede);
        }
    }


    public void OnSceneLoadStart(NetworkRunner runner) {}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        salasDisponiveis = sessionList;
        listaLobby.text = "";

        if (salasDisponiveis.Count == 0)
        {
            listaLobby.text = "Nenhuma sala disponível.";
            return;
        }

        foreach (var session in salasDisponiveis)
        {
            listaLobby.text += $"Sala: {session.Name} | Jogadores: {session.PlayerCount}/{session.MaxPlayers}\n";
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
}
