using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public RedeNeural nn;

    public int distance;
    public int generation;
    public float jumpForce;

    private float jumpTimer;
    
    protected float fallMultiplier = 7.5f;
	protected float lowJumpMultiplier = 5f;

    public bool canMove;

    private Rigidbody2D rb;

    private GameManager gameManager;

    void Awake() {
        nn = new RedeNeural(2, 3, 1);
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start() {
        jumpTimer = 0;
        distance = 0;
        canMove = true;
    }

    // Update is called once per frame
    void Update() {
        if (canMove) {
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
            /*if (PlayerPrefs.HasKey("Distance")) {
                if (PlayerPrefs.GetFloat("Distance") < distance) {
                    PlayerPrefs.SetFloat("Distance", distance);
                }
            } else {
                PlayerPrefs.SetFloat("Distance", distance);
            }*/

            if (distance > gameManager.bestBirds[0].distance) {
                gameManager.bestBirds[0] = this;
                gameManager.bestBirdsNN[0] = nn;
                Debug.Log("Oii1");
            } else if (distance > gameManager.bestBirds[1].distance) {
                gameManager.bestBirds[1] = this;
                gameManager.bestBirdsNN[1] = nn;
                Debug.Log("Oii2");
            }

            distance = 0;

            gameManager.birdsCount++;
            if (gameManager.birdsCount >= 99) {
                gameManager.RestartGame();
            }

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D coll) {
        if (coll.transform.tag == "Point") {
            distance++;
        }
    }
}
