using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class InferenceComparer : MonoBehaviour
{
    [Header("Configuração")]
    public ModelAsset mobileNetModel;

    [Header("Imagens para Teste")]
    public Texture2D originalDrawing;
    public List<Texture2D> playersDrawing;

    public List<Player> players;
    private Model runtimeModel;
    private Worker worker;

    void Start()
    {
        runtimeModel = ModelLoader.Load(mobileNetModel);

        worker = new Worker(runtimeModel, BackendType.GPUCompute);

        players = FindObjectsByType<Player>(FindObjectsSortMode.None).OrderBy(player => player.GetID()).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Compare());
        }
    }

    private System.Collections.IEnumerator Compare()
    {
        foreach (var player in players)
        {
            playersDrawing.Add(player.GetDrawing());
        }

        foreach (var playerDrawing in playersDrawing)
        {
            float similarity = CompareDrawings(originalDrawing, playerDrawing);

            var player = players[playersDrawing.IndexOf(playerDrawing)];

            player.AddScore(similarity);
            //Debug.Log($"Player {player.id}: {similarity * 100:F0} Pontos!");
        }

        yield return new WaitForEndOfFrame();

        playersDrawing.Clear();

        //string playersInfo = string.Join(", ", players.Select(p => $"Player {p.GetID()}"));
        //string drawingsInfo = string.Join(", ", playersDrawing.Select(d => d.name));

        //Debug.Log(playersInfo.ToString());
        //Debug.Log(drawingsInfo.ToString());
    }

    float CompareDrawings(Texture2D texA, Texture2D texB)
    {
        using Tensor<float> inputA = new Tensor<float>(new TensorShape(1, 3, 224, 224));
        using Tensor<float> inputB = new Tensor<float>(new TensorShape(1, 3, 224, 224));

        TextureConverter.ToTensor(texA, inputA);
        TextureConverter.ToTensor(texB, inputB);

        worker.Schedule(inputA);
        using Tensor<float> outputA = worker.PeekOutput() as Tensor<float>;
        float[] featuresA = outputA.DownloadToArray();

        worker.Schedule(inputB);
        using Tensor<float> outputB = worker.PeekOutput() as Tensor<float>;
        float[] featuresB = outputB.DownloadToArray();

        return CosineSimilarity(featuresA, featuresB);
    }

    float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        float dotProduct = 0f;
        float magnitudeA = 0f;
        float magnitudeB = 0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0) return 0;
        return dotProduct / (Mathf.Sqrt(magnitudeA) * Mathf.Sqrt(magnitudeB));
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}