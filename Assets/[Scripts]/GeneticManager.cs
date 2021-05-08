using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class GeneticManager : MonoBehaviour {
    [Header("References")]
    public CarBrain carBrain;

    [Header("Controls")]
    public int initialPopulation = 85;
    [Range(0f, 1f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Controls")]
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 3;
    public int numberToCrossover;

    private List<int> genePool = new List<int>();

    private int naturallySelected;

    private NNet[] population;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome = 0;

    #region Singleton
    public static GeneticManager instance = null;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            DestroyImmediate(this);
        }
    }
    #endregion

    private void Start() {
        CreatePopulation();
    }

    private void CreatePopulation() {
        population = new NNet[initialPopulation];
        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome() {
        carBrain.ResetWithNetwork(population[currentGenome]);
    }

    // Some nets up to startingIndex may already been initialized with crossover.
    // We may just have to initialize only the remaining population with random values.
    private void FillPopulationWithRandomValues(NNet[] newPopulation, int startingIndex) {
        while (startingIndex < initialPopulation) {
            newPopulation[startingIndex] = new NNet();
            newPopulation[startingIndex].Initialize(carBrain.layers, carBrain.neurons);
            startingIndex++;
        }
    }

    public void Death(float fitness, NNet network) {
        if (currentGenome < population.Length - 1) {
            population[currentGenome].fitness = fitness;
            currentGenome++;
            ResetToCurrentGenome();
        } else {
            Repopulate();
        }
    }

    private void Repopulate() {
        genePool.Clear();
        currentGeneration++;
        naturallySelected = 0;
        SortPopulation();

        NNet[] newPopulation = PickBestPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        population = newPopulation;

        currentGenome = 0;

        ResetToCurrentGenome();
    }

    private void Crossover(NNet[] newPopulation) {
        for (int i = 0; i < numberToCrossover; i += 2) {
            // These are indexes of the parents.
            int indexA = i;
            int indexB = i + 1;

            if (genePool.Count >= 1) {
                // We need to ensure that these are not the same.
                // Because if two parents are the same, we're not gonna get a new child.
                indexA = genePool[Random.Range(0, genePool.Count)];
                do
                    indexB = genePool[Random.Range(0, genePool.Count)];
                while (indexA == indexB);
            }

            NNet child1 = new NNet();
            NNet child2 = new NNet();

            child1.Initialize(carBrain.layers, carBrain.neurons);
            child2.Initialize(carBrain.layers, carBrain.neurons);

            child1.fitness = 0;
            child2.fitness = 0;

            for (int w = 0; w < child1.weights.Count; w++) {
                if (Random.Range(0f, 1f) < 0.5f) {
                    child1.weights[w] = population[indexA].weights[w];
                    child2.weights[w] = population[indexB].weights[w];
                } else {
                    child1.weights[w] = population[indexB].weights[w];
                    child2.weights[w] = population[indexA].weights[w];
                }
            }

            for (int b = 0; b < child1.biases.Count; b++) {
                if (Random.Range(0f, 1f) < 0.5f) {
                    child1.biases[b] = population[indexA].biases[b];
                    child2.biases[b] = population[indexB].biases[b];
                } else {
                    child1.biases[b] = population[indexB].biases[b];
                    child2.biases[b] = population[indexA].biases[b];
                }
            }

            newPopulation[naturallySelected] = child1;
            naturallySelected++;

            newPopulation[naturallySelected] = child2;
            naturallySelected++;
        }

    }

    private void Mutate(NNet[] newPopulation) {
        for (int i = 0; i < naturallySelected; i++) {

            for (int c = 0; c < newPopulation[i].weights.Count; c++) {

                if (Random.Range(0f, 1f) < mutationRate) {
                    newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                }

            }

        }
    }

    private Matrix<float> MutateMatrix(Matrix<float> mat) {
        int randomPoints = Random.Range(1, (mat.RowCount * mat.ColumnCount) / 7);

        Matrix<float> temp = mat;
        
        for (int i = 0; i < randomPoints; i++) {
            int randomColumn = Random.Range(0, temp.ColumnCount);
            int randomRow = Random.Range(0, temp.RowCount);

            temp[randomRow, randomColumn] = Mathf.Clamp(temp[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return temp;
    }

    private NNet[] PickBestPopulation() {
        NNet[] newPopulation = new NNet[initialPopulation];

        for (int i = 0; i < bestAgentSelection; i++) {
            newPopulation[naturallySelected] = population[i].InitializeCopy(carBrain.layers, carBrain.neurons);
            newPopulation[naturallySelected].fitness = 0;
            naturallySelected++;

            // Value of fitness * 10 indicates how many times we're going to add this current network to the genePool.
            // This means, better fitness value will increase the chance of being selected for crossover.
            int f = Mathf.RoundToInt(population[i].fitness * 10);

            for (int c = 0; c < f; c++) {
                // Instead of adding nets to the gene pool, we only keep the indexes. Thus, we do not keep unnecessary references.
                genePool.Add(i);
            }
        }

        for (int i = 0; i < worstAgentSelection; i++) {
            int last = population.Length - 1;
            last -= i;

            // Same logic as selecting best agents seen above.
            int f = Mathf.RoundToInt(population[last].fitness * 10);

            for (int c = 0; c < f; c++) {
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

    private void SortPopulation() {
        // Bubblesort (can be replaced for performance)
        for (int i = 0; i < population.Length; i++) {
            for (int j = 0; j < population.Length; j++) {
                if (population[i].fitness < population[j].fitness) {
                    NNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }
    }
}
