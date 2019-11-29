using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public RedeNeural nn;

	public float fitness;
	public float score;
    public float distance; //Fitness
    public int generation;
    public float jumpForce;

    private float jumpTimer;
    
    protected float fallMultiplier = 7.5f;
	protected float lowJumpMultiplier = 5f;

    public bool canMove;

    private Rigidbody2D rb;

    private GameManager gameManager;

    void Awake() {
        nn = new RedeNeural(2, 6, 1);
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start() {
        jumpTimer = 0;
		fitness = 0;
		score = 0;
		distance = 0;
        canMove = true;
    }

    // Update is called once per frame
    void Update() {
        if (canMove) {
			if (transform.position.y < -10f) {
				fitness = score / distance;

				gameManager.birdsCount++;
				if (gameManager.birdsCount == 100) {
					gameManager.RestartGame();
				}

				GetComponent<SpriteRenderer>().enabled = false;
				GetComponent<CircleCollider2D>().enabled = false;
				enabled = false;
			}

			distance += GameManager.gameSpeed;
            SetGravity();
            SetVelocity();
        }
    }

    public void Jump() {
        // if (Input.GetKeyDown(KeyCode.Space)) {
            jumpTimer += Time.deltaTime;

            if (jumpTimer > 0.1f) {
                if (transform.position.y < 4) {
                    rb.velocity += new Vector2(rb.velocity.x, 0);
                    rb.velocity += Vector2.up * jumpForce;
                }

                jumpTimer = 0;
            }
        // }
    }

    private void SetGravity() {
		if (rb.velocity.y <= 0) {
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		} else if (rb.velocity.y > 0) {
			rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
	}

    private void SetVelocity () {
        if (rb.velocity.y > 12) {
            rb.velocity = new Vector2(0, 12);
        } else if (rb.velocity.y < -12) {
            rb.velocity = new Vector2(0, -12);
        }
    }

    private void OnTriggerEnter2D(Collider2D coll) {
        if ((coll.transform.parent.tag == "Pipe" && (coll.transform.name == "Up" || coll.transform.name == "Down")) ||
            coll.transform.tag == "Fall") {
            //fitness = score * distance;
            fitness = distance - ((GameManager.pipes[0].transform.GetChild(2).transform.position.x - transform.position.x) + Mathf.Abs(GameManager.pipes[0].transform.GetChild(2).transform.position.y - transform.position.y));

            if (PlayerPrefs.HasKey("BestFitness")) {
                if (fitness > PlayerPrefs.GetFloat("BestFitness")) {
                    PlayerPrefs.SetFloat("BestFitness", fitness);
                    gameManager.bestBirdEver = this;
                }
            } else {
                PlayerPrefs.SetFloat("BestFitness", fitness);
            }

			GetComponent<SpriteRenderer>().enabled = false;
			GetComponent<CircleCollider2D>().enabled = false;
			enabled = false;

			gameManager.birdsCount++;
			if (gameManager.birdsCount == 100) {
				gameManager.RestartGame();
			}
		}

		if (coll.transform.tag == "Point") {
			score++;
			gameManager.DisplayScore();
		}
	}
}
