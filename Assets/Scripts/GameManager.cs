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
    public RedeNeural bestBird;

    // Start is called before the first frame update
    void Start() {
        gameSpeed = 10f;
        train = true;
        timer = 2f;
        birdsCount = 0;

        bestBird = new RedeNeural(2, 3, 1);
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

                if (birdNN.predict(input)[0] > 0.0d) {
                    if (i == 0) {
                        Debug.Log("Jump");
                    }
                    birds[i].GetComponent<Player>().Jump();
                } else {
                    Debug.Log("Oii");
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

        for (int i = 0; i < pipes.Count; i++) {
            Destroy(pipes[i]);
            i--;
        }

        GameObject[] pipesTemp = GameObject.FindGameObjectsWithTag("Pipe");

        for (int i = 0; i < pipesTemp.Length; i++) {
            Destroy(pipesTemp[i]);
        }

        for (int i = 0; i < birds.Count; i++) {
            Destroy(birds[i]);
            birds.Remove(birds[i]);
        }

        GenerateBirds();
    }

    private void GenerateBirds() {
        for (int i = 0; i < 100; i++) {
            GameObject bird = Instantiate(birdPrefab);

            RedeNeural birdNN = bird.GetComponent<Player>().nn;
            birdNN = bestBird;

            //Matrix.randomizeWeights(birdNN.weights_ih);
            //Matrix.randomizeWeights(birdNN.weights_ho);
            birdNN.weights_ih.randomize();
            birdNN.weights_ho.randomize();
            birdNN.bias_ih.randomize();
            birdNN.bias_ho.randomize();

            bird.transform.parent = birdsTransform;

            bird.transform.position = new Vector3(Random.Range(-8f, -5f), Random.Range(-2f, 2f), 0);
            birds.Add(bird);
        }
    }
}
