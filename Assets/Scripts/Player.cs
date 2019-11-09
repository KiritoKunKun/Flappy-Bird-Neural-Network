using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int score;
    public int losses;
    public int generation;

    private GameManager gameManager;

    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start() {
        gameManager.player = transform;
        score = 0;
        losses = 0;
    }

    // Update is called once per frame
    void Update() {
        PlayerMovement();
    }

    private void PlayerMovement() {
        if (Input.GetKey(KeyCode.RightArrow)) {
            MoveRight();
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            MoveLeft();
        }
    }

    public void MoveRight() {
        if (transform.position.x < 6) {
            transform.Translate(new Vector3(10 * Time.deltaTime, 0, 0));
        }
    }

    public void MoveLeft() {
        if (transform.position.x > -6) {
            transform.Translate(new Vector3(-10 * Time.deltaTime, 0, 0));
        }
    }
}
