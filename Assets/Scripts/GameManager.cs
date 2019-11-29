using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static float gameSpeed;

    public bool train;

    private float timer;

    private double[][] inputs;

    private double[][] outputs;

    public GameObject pipePrefab;
    public GameObject birdPrefab;

    public GameObject player;

    public Transform pipesTransform;
    public Transform birdsTransform;

    public static List<GameObject> pipes = new List<GameObject>();
    public static List<GameObject> birds = new List<GameObject>();
    public int birdsCount;
    public Player bestBirdEver;
    public List<Player> bestBirds;
    public List<RedeNeural> bestBirdsNN;
	public float[] totalDistances;
	public float[] totalScores;
	public float[] totalFitness;

	public Text scoreText;

    // Start is called before the first frame update
    void Start() {
        gameSpeed = 5f;
        train = true;
        timer = 2f;
        birdsCount = 0;

        bestBirdEver = new Player();

        bestBirds = new List<Player>();
        bestBirds.Add(new Player());
		bestBirds.Add(new Player());
		bestBirds.Add(new Player());
		bestBirds.Add(new Player());

		bestBirdsNN = new List<RedeNeural>();
        bestBirdsNN.Add(new RedeNeural(2, 6, 1));
		bestBirdsNN.Add(new RedeNeural(2, 6, 1));
		bestBirdsNN.Add(new RedeNeural(2, 6, 1));
		bestBirdsNN.Add(new RedeNeural(2, 6, 1));
        
		totalDistances = new float[100];
		totalScores = new float[100];
		totalFitness = new float[100];

        GenerateBirds();
    }

    // Update is called once per frame
    void Update() {
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

	public void DisplayScore() {
        /*birds.Sort((a, b) => { return a.GetComponent<Player>().score.CompareTo(b.GetComponent<Player>().score); });
		for (int i = 0; i < birds.Count; i++) {
			scoreText.text = "Score: " + birds[birds.Count - 1].GetComponent<Player>().score;
		}*/

        scoreText.text = "Score: " + bestBirdEver.score;
    }

    public void DisplayBestBird() {
        scoreText.text = "Best Fitness: " + (int)bestBirdEver.fitness;
    }

	private void CheckBestFitness() {
		int[] indexes = new int[4];
		float[] bestFitness = new float[4];

		for (int i = 0; i < birds.Count; i++) {
            totalFitness[i] = birds[i].GetComponent<Player>().fitness;
        }

		for (int i = 0; i < birds.Count; i++) {
			totalDistances[i] = birds[i].GetComponent<Player>().distance;
			totalScores[i] = birds[i].GetComponent<Player>().score;
		}
		
		Array.Sort(totalFitness);
		birds.Sort((a, b) => { return a.GetComponent<Player>().fitness.CompareTo(b.GetComponent<Player>().fitness); });

		for (int i = 0; i < bestFitness.Length; i++) {
			bestFitness[i] = totalFitness[totalFitness.Length - 1 - i];
		}

		//Clear Birds
		for (int i = 0; i < bestBirds.Count; i++) {
			bestBirds.RemoveAt(i);
			i--;
		}

		for (int i = 0; i < bestBirdsNN.Count; i++) {
			bestBirdsNN.RemoveAt(i);
			i--;
		}

		for (int i = birdsCount - 5; i < birds.Count; i++) {
			bestBirds.Add(birds[i].GetComponent<Player>());
			bestBirdsNN.Add(birds[i].GetComponent<Player>().nn);
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
        DisplayBestBird();

        birdsCount = 0;

        for (int i = 0; i < pipes.Count; i++) {
            pipes.RemoveAt(i);
			i--;
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
		for (int i = 0; i < birds.Count; i++) {
			bool canMutate = true;

            birds[i].transform.position = new Vector3(UnityEngine.Random.Range(-7f, -4f), UnityEngine.Random.Range(-2f, 2f), 0);
            birds[i].GetComponent<SpriteRenderer>().enabled = true;
            birds[i].GetComponent<CircleCollider2D>().enabled = true;
            birds[i].GetComponent<Player>().enabled = true;
            birds[i].GetComponent<Player>().distance = 0;
			birds[i].GetComponent<Player>().score = 0;
			birds[i].GetComponent<Player>().fitness = 0;

			for (int j = 0; j < bestBirds.Count; j++) {
				if (birds[i] == bestBirds[j]) {
					canMutate = false;
				}
			}

			if (canMutate) {
				RedeNeural birdNN = birds[i].GetComponent<Player>().nn;

				float rnd = UnityEngine.Random.Range(0f, 100f);

                int rnd1 = UnityEngine.Random.Range(0, 2);
                int rnd2 = UnityEngine.Random.Range(0, 2);

                while (rnd2 == rnd1) {
                    rnd2 = UnityEngine.Random.Range(0, 2);
                }

                if (rnd < 70f) {
					birdNN = bestBirdsNN[0];
				} else if (rnd >= 70f && rnd <= 73f) {
                    //Mutation
                    float rndMut = UnityEngine.Random.Range(0, 100);

                    if (rndMut < 70f) {
                        birdNN.weights_ih = Matrix.mutation(bestBirdsNN[rnd1].weights_ih, bestBirdsNN[rnd2].weights_ih);
                        birdNN.weights_ho = Matrix.mutation(bestBirdsNN[rnd1].weights_ho, bestBirdsNN[rnd2].weights_ho);
                    } else {
                        birdNN.weights_ih = Matrix.mutation(bestBirdsNN[0].weights_ih, bestBirdsNN[1].weights_ih);
                        birdNN.weights_ho = Matrix.mutation(bestBirdsNN[0].weights_ho, bestBirdsNN[1].weights_ho);
                    }
                } else {
                    //Crossover
                    float rndCross = UnityEngine.Random.Range(0, 100);

                    if (rndCross < 70f) {
                        birdNN.weights_ih = Matrix.crossover(bestBirdsNN[0].weights_ih, bestBirdsNN[1].weights_ih);
                        birdNN.weights_ho = Matrix.crossover(bestBirdsNN[0].weights_ho, bestBirdsNN[1].weights_ho);
                    } else {
                        birdNN.weights_ih = Matrix.crossover(bestBirdsNN[rnd1].weights_ih, bestBirdsNN[rnd2].weights_ih);
                        birdNN.weights_ho = Matrix.crossover(bestBirdsNN[rnd1].weights_ho, bestBirdsNN[rnd2].weights_ho);
                    }
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
