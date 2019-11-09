using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour {

    private int generation;

    public List<float> ballsX;
    public List<GameObject> balls;

    public GameObject ball;
    public Transform gameObjects;
    public GameObject playerPrefab;
    private GameManager gameManager;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start() {
        generation = 0;
        ballsX = new List<float>();
        spawnBall();
    }

    public void spawnBall() {
        GameObject b = Instantiate(ball, new Vector3(0, 0, 0), Quaternion.identity);
        b.transform.parent = gameObjects;
        
        float rnd = Random.Range(-4f, 4f);
        ballsX.Add(rnd);
        b.transform.position = new Vector3(rnd, 10, 0);
        balls.Add(b);
    }

    public void spawnPlayer() {
        generation++;

        GameObject pl = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pl.transform.parent = gameObjects;
        pl.transform.position = new Vector3(0, -3.5f, 0);
        pl.GetComponent<Player>().generation = generation;

        Destroy(gameManager.player.gameObject);
        gameManager.player = pl.transform;

        Debug.Log(generation);
        
        for (int i = 0; i < balls.Count; i++) {
            Destroy(balls[i]);
            balls.Remove(balls[i]);
            i--;
        }
    }
}
