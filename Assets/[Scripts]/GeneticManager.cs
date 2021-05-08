using System.Collections.Generic;
using UnityEngine;

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
        if (currentGeneration < population.Length - 1) {
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
