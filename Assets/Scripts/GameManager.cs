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
    public double[] data00;
    public double[] data01;

    // Start is called before the first frame update
    void Start() {
        gameSpeed = 5f;
        train = true;
        timer = 2f;
        birdsCount = 0;

        bestBirds = new Player[2];
        bestBirds[0] = new Player();
        bestBirds[1] = new Player();

        bestBirdsNN = new RedeNeural[2];
        bestBirdsNN[0] = new RedeNeural(2, 3, 1);
        bestBirdsNN[1] = new RedeNeural(2, 3, 1);

        nn = new RedeNeural(2, 3, 1);

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

        if (pipes.Count > 0) {
            for (int i = 0; i < birds.Count; i++) {
                RedeNeural birdNN = birds[i].GetComponent<Player>().nn;

                double distX = pipes[0].transform.position.x - birds[i].transform.position.x;
                double distY = pipes[0].transform.position.y - birds[i].transform.position.y;

                double[] input = new double[2];
                double[] output = new double[1];

                input[0] = distX;
                input[1] = distY;

                if (birdNN.predict(input)[0] > 0.85d) {
                    if (i == 0) {
                    }
                    birds[i].GetComponent<Player>().Jump();
                }
            }
        }
    }

    private void GeneratePipes() {
        timer += Time.deltaTime;

        if (timer > 2f) {
            GameObject pipe = Instantiate(pipePrefab);
            pipe.transform.parent = pipesTransform;

            float rndY = Random.Range(-2f, 2f);
            pipe.transform.position = new Vector3(10, rndY, 0);
            pipes.Add(pipe);

            timer = 0f;
        }
    }

    public void RestartGame() {
        birdsCount = 0;

        if (bestBirds[0] == null || bestBirds[0] == birdPrefab) {
            bestBirds[0] = birdPrefab.GetComponent<Player>();
            bestBirds[1] = birdPrefab.GetComponent<Player>();

            bestBirds[0].nn = new RedeNeural(2, 3, 1);
            bestBirds[1].nn = new RedeNeural(2, 3, 1);

            bestBirds[0].nn.weights_ih.randomize();
            bestBirds[1].nn.weights_ih.randomize();
            bestBirds[0].nn.weights_ho.randomize();
            bestBirds[1].nn.weights_ho.randomize();
        }

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
        for (int i = 0; i < birds.Count; i++) {
            birds[i].transform.position = new Vector3(Random.Range(-7f, -4f), Random.Range(-2f, 2f), 0);
            birds[i].GetComponent<SpriteRenderer>().enabled = true;
            birds[i].GetComponent<CircleCollider2D>().enabled = true;
            birds[i].GetComponent<Player>().enabled = true;
            birds[i].GetComponent<Player>().distance = 0;

            RedeNeural birdNN = birds[i].GetComponent<Player>().nn;

            //Mutation
            birdNN.weights_ih = Matrix.mutation(bestBirdsNN[0].weights_ih, bestBirdsNN[1].weights_ih);
            birdNN.weights_ho = Matrix.mutation(bestBirdsNN[0].weights_ho, bestBirdsNN[1].weights_ho);
            //birdNN.bias_ih = Matrix.mutation(bestBirdsNN[0].bias_ih, bestBirdsNN[1].bias_ih);
            //birdNN.bias_ho = Matrix.mutation(bestBirdsNN[0].bias_ho, bestBirdsNN[1].bias_ho);
        }
    }

    private void GenerateBirds() {
        for (int i = 0; i < 100; i++) {
            GameObject bird = Instantiate(birdPrefab);

            RedeNeural birdNN = bird.GetComponent<Player>().nn;

            //Mutation
            birdNN.weights_ih = Matrix.mutation(bestBirdsNN[0].weights_ih, bestBirdsNN[1].weights_ih);
            birdNN.weights_ho = Matrix.mutation(bestBirdsNN[0].weights_ho, bestBirdsNN[1].weights_ho);
            birdNN.bias_ih = Matrix.mutation(bestBirdsNN[0].bias_ih, bestBirdsNN[1].bias_ih);
            birdNN.bias_ho = Matrix.mutation(bestBirdsNN[0].bias_ho, bestBirdsNN[1].bias_ho);

            bird.transform.parent = birdsTransform;
            bird.transform.position = new Vector3(Random.Range(-8f, -5f), Random.Range(-2f, 2f), 0);
            birds.Add(bird);
        }
    }
}
