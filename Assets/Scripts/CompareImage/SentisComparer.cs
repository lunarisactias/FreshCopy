using UnityEngine;
using Unity.InferenceEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class SentisComparer : MonoBehaviour
{
    public static SentisComparer Instance;

    [Header("Configuração")]
    public ModelAsset mobileNetModel;

    [Header("Imagens para Teste")]
    public Texture2D originalDrawing;

    private Model runtimeModel;
    private Worker worker;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        runtimeModel = ModelLoader.Load(mobileNetModel);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);
    }

    public void EvaluateLocalPlayer(Player localPlayer)
    {
        StartCoroutine(CompareRoutine(localPlayer));
    }

    IEnumerator CompareRoutine(Player localPlayer)
    {
        while (localPlayer.GetDrawing() == null)
        {
            yield return null; 
        }

        Texture2D texPlayer = localPlayer.GetDrawing();

        if (texPlayer == null)
        {
            float similarity = CompareDrawings(originalDrawing, texPlayer);
            localPlayer.AddScore(similarity);
        }
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