using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;
using Random = UnityEngine.Random;

public class NNet : MonoBehaviour {
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 3);

    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

    public List<Matrix<float>> weights = new List<Matrix<float>>();

    public List<float> biases = new List<float>();

    public float fitness;

    public void Initialize(int hiddenLayerCount, int hiddenNeuronCount) {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++) {
            // Initialize a new hidden layer
            Matrix<float> newLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(newLayer);

            // Adding random biases
            biases.Add(Random.Range(-1f, 1f));

            // Weights
            if (i == 0) {
                Matrix<float> inputToFirstHidden = Matrix<float>.Build.Dense(3, hiddenNeuronCount);
                weights.Add(inputToFirstHidden);
            }

            Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(hiddenToHidden);
        }

        Matrix<float> outputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
        weights.Add(outputWeight);

        biases.Add(Random.Range(-1f, 1f));

        RandomizeWeights();
    }

    public void RandomizeWeights() {
        for (int i = 0; i < weights.Count; i++) {
            for (int x = 0; x < weights[i].RowCount; x++) {
                for (int y = 0; y < weights[i].ColumnCount; y++) {
                    weights[i][x, y] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public (float, float) RunNetwork (float a, float b, float c) {
        inputLayer[0, 0] = a;
        inputLayer[0, 1] = b;
        inputLayer[0, 2] = c;

        inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++) {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        // First output is throttle, second output is steering
        return (Sigmoid(outputLayer[0, 0]), (float) Math.Tan(outputLayer[0, 1]));
    }

    // We wrote our own Sigmoid function
    private float Sigmoid(float s) {
        return (1 / (1 + Mathf.Exp(-s)));
    }
}
