using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static float gameSpeed;

    public bool train;

    private float timer;

    private double[][] inputs;

    private double[][] outputs;

    public RedeNeural nn;

    public GameObject pipePrefab;
    public GameObject birdPrefab;

    public GameObject player;

    public Transform pipesTransform;
    public Transform birdsTransform;

    public static List<GameObject> pipes = new List<GameObject>();
    public static List<GameObject> birds = new List<GameObject>();
    public int birdsCount;
    public Player[] bestBirds;
    public RedeNeural[] bestBirdsNN;
	public float[] bestDistances;
	public float[] totalDistances;
	public float[] totalScores;
	public float[] totalFitness;
	public double[] data00;
    public double[] data01;

    // Start is called before the first frame update
    void Start() {
        gameSpeed = 5f;
        train = true;
        timer = 2f;
        birdsCount = 0;

        bestBirds = new Player[4];
        bestBirds[0] = new Player();
        bestBirds[1] = new Player();
		bestBirds[2] = new Player();
		bestBirds[3] = new Player();

		bestBirdsNN = new RedeNeural[4];
        bestBirdsNN[0] = new RedeNeural(2, 6, 1);
        bestBirdsNN[1] = new RedeNeural(2, 6, 1);
		bestBirdsNN[2] = new RedeNeural(2, 6, 1);
		bestBirdsNN[3] = new RedeNeural(2, 6, 1);

		bestDistances = new float[4] { 0, 0, 0, 0 };
		totalDistances = new float[100];
		totalScores = new float[100];
		totalFitness = new float[100];

		nn = new RedeNeural(2, 6, 1);

        //XOR Problem
        
        inputs = new double[][]
        {
            new double[] { 0, 0},
            new double[] { 0, 1},
            new double[] { 1, 0},
            new double[] { 1, 1}
        };

        outputs = new double[][] 
        { 
            new double[] {0},
            new double[] {1},
            new double[] {1},
            new double[] {0}
        };

        GenerateBirds();
    }

    // Update is called once per frame
    void Update() {
        data00 = bestBirdsNN[0].weights_ih.data[0];
        data01 = bestBirdsNN[0].weights_ih.data[1];
        GeneratePipes();

        if (pipes.Count > 0 && pipes[0] != null) {
            for (int i = 0; i < birds.Count; i++) {
                RedeNeural birdNN = birds[i].GetComponent<Player>().nn;

                double distX = pipes[0].transform.position.x - birds[i].transform.position.x;
                double distY = pipes[0].transform.position.y - birds[i].transform.position.y;

                double[] input = new double[2];
                double[] output = new double[1];

                input[0] = distX;
                input[1] = distY;

                if (birdNN.predict(input)[0] > 0.5d) {
                    birds[i].GetComponent<Player>().Jump();
                }
            }
        }
    }

	private void CheckBestFitness() {
		int[] indexes = new int[4];
		float[] bestFitness = new float[4];

		for (int i = 0; i < birds.Count; i++) {
			totalFitness[i] = totalScores[i] / totalDistances[i];
		}

		for (int i = 0; i < birds.Count; i++) {
			totalDistances[i] = birds[i].GetComponent<Player>().distance;
			totalScores[i] = birds[i].GetComponent<Player>().score;
		}
		
		Array.Sort(totalFitness);
		birds.Sort((a, b) => { return a.GetComponent<Player>().fitness.CompareTo(b.GetComponent<Player>().fitness); });

		for (int i = 0; i < bestFitness.Length; i++) {
			if (totalFitness[totalFitness.Length - 1 - i] > bestDistances[i]) {
				bestFitness[i] = totalFitness[totalFitness.Length - 1 - i];
				bestDistances[i] = bestFitness[i];
			}
		}
		
		for (int i = birdsCount - 5; i < birds.Count; i++) {
			for (int j = 0; j < bestFitness.Length; j++) {
				if (birds[i].GetComponent<Player>().fitness > bestBirds[j].fitness) {
					bestBirds[j] = birds[i].GetComponent<Player>();
					bestBirdsNN[j] = birds[i].GetComponent<Player>().nn;
					break;
				}
			}
		}
	}

    private void GeneratePipes() {
        timer += Time.deltaTime;

        if (timer > 2f) {
            GameObject pipe = Instantiate(pipePrefab);
            pipe.transform.parent = pipesTransform;

            float rndY = UnityEngine.Random.Range(-2f, 2f);
            pipe.transform.position = new Vector3(10, rndY, 0);
            pipes.Add(pipe);

            timer = 0f;
        }
    }

    public void RestartGame() {
		CheckBestFitness();

		birdsCount = 0;

        for (int i = 0; i < pipes.Count; i++) {
            pipes.RemoveAt(i);
        }

        GameObject[] pipesTemp = GameObject.FindGameObjectsWithTag("Pipe");
        for (int i = 0; i < pipesTemp.Length; i++) {
            Destroy(pipesTemp[i]);
        }

        timer = 2f;
        GeneratePipes();

        RespawnBirds();
    }

    private void RespawnBirds() {
		for (int i = 0; i < bestDistances.Length; i++) {
			//Debug.Log("Best " + i + ": " + bestDistances[i]);
		}

		for (int i = 0; i < birds.Count; i++) {
			bool canMutate = true;

            birds[i].transform.position = new Vector3(UnityEngine.Random.Range(-7f, -4f), UnityEngine.Random.Range(-2f, 2f), 0);
            birds[i].GetComponent<SpriteRenderer>().enabled = true;
            birds[i].GetComponent<CircleCollider2D>().enabled = true;
            birds[i].GetComponent<Player>().enabled = true;
            birds[i].GetComponent<Player>().distance = 0;
			birds[i].GetComponent<Player>().score = 0;
			birds[i].GetComponent<Player>().fitness = 0;

			for (int j = 0; j < bestBirds.Length; j++) {
				if (birds[i] == bestBirds[j]) {
					canMutate = false;
				}
			}

			if (canMutate) {
				RedeNeural birdNN = birds[i].GetComponent<Player>().nn;

				int rnd = UnityEngine.Random.Range(0, 5);

				if (rnd == 0) {
					birdNN.weights_ih.randomize();
					birdNN.weights_ho.randomize();
					birdNN.bias_ih.randomize();
					birdNN.bias_ho.randomize();
				} else {
					int rnd1 = UnityEngine.Random.Range(0, 4);
					int rnd2 = UnityEngine.Random.Range(0, 4);

					while (rnd2 == rnd1) {
						rnd2 = UnityEngine.Random.Range(0, 4);
					}

					//Crossover
					birdNN.weights_ih = Matrix.crossover(bestBirdsNN[rnd1].weights_ih, bestBirdsNN[rnd2].weights_ih);
					birdNN.weights_ho = Matrix.crossover(bestBirdsNN[rnd1].weights_ho, bestBirdsNN[rnd2].weights_ho);
					//birdNN.bias_ih = Matrix.crossover(bestBirdsNN[rnd1].bias_ih, bestBirdsNN[rnd2].bias_ih);
					//birdNN.bias_ho = Matrix.crossover(bestBirdsNN[rnd1].bias_ho, bestBirdsNN[rnd2].bias_ho);

					//Mutation
					birdNN.weights_ih = Matrix.mutation(bestBirdsNN[rnd1].weights_ih, bestBirdsNN[rnd2].weights_ih);
					birdNN.weights_ho = Matrix.mutation(bestBirdsNN[rnd1].weights_ho, bestBirdsNN[rnd2].weights_ho);
					//birdNN.bias_ih = Matrix.escalarMultiply(birdNN.weights_ih, UnityEngine.Random.Range(0f, 1f));
					//birdNN.bias_ho = Matrix.escalarMultiply(birdNN.weights_ho, UnityEngine.Random.Range(0f, 1f));
				}
			}
        }
    }

    private void GenerateBirds() {
        for (int i = 0; i < 100; i++) {
            GameObject bird = Instantiate(birdPrefab);

            RedeNeural birdNN = bird.GetComponent<Player>().nn;
			birdNN.weights_ih.randomize();
			birdNN.weights_ho.randomize();
			birdNN.bias_ih.randomize();
			birdNN.bias_ho.randomize();

			bird.transform.parent = birdsTransform;
            bird.transform.position = new Vector3(UnityEngine.Random.Range(-8f, -5f), UnityEngine.Random.Range(-2f, 2f), 0);
            birds.Add(bird);
        }
    }
}
