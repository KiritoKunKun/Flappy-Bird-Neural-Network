using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static float gameSpeed;

    public bool train;

    private float timer;

    private double[][] inputs;

    private double[][] outputs;

    private RedeNeural nn;

    public GameObject pipePrefab;

    public GameObject player;
    public Transform pipesTransform;
    public static List<GameObject> pipes = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        gameSpeed = 10f;
        train = true;
        timer = 2f;

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

		// for (int i = 0; i < 5000; i++) {
		// 	for (int index = 0; index < 2; index ++) {
		// 		nn.train(inputs[index], outputs[index]);
		// 	}
		// }

		// for (int i = 0; i < 4; i++) {
		// 	Debug.Log("0 = " + nn.predict(new double[] { 0, 0 })[0]);
		// 	Debug.Log("1 = " + nn.predict(inputs[3])[0]);
		// }
	}

    // Update is called once per frame
    void Update() {
        GeneratePipes();

        if (pipes.Count > 0) {
            double distX = pipes[0].transform.position.x - player.transform.position.x;
            double distY = pipes[0].transform.position.y - player.transform.position.y;

            double[] input = new double[2];
            double[] output = new double[1];

            input[0] = distX;
            input[1] = distY;
            
            if (nn.predict(input)[0] > 0.0d) {
                player.GetComponent<Player>().Jump();
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
        for (int i = 0; i < pipes.Count; i++) {
            Destroy(pipes[i]);
            i--;
        }

        player.transform.position = new Vector3(-7, 0, 0);
    }
}
