using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public float distance;
    public int generation;
    public float jumpForce;
    
    protected float fallMultiplier = 7.5f;
	protected float lowJumpMultiplier = 5f;

    private Rigidbody2D rb;

    private GameManager gameManager;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start() {
        gameManager.player = transform;
        distance = 0;
    }

    // Update is called once per frame
    void Update() {
        distance += Time.deltaTime;
        SetGravity();
        Jump();
        SetVelocity();
    }

    public void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.velocity += new Vector2(rb.velocity.x, 0);
			rb.velocity += Vector2.up * jumpForce;
        }
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
        if (coll.transform.parent.tag == "Pipe") {
            if (PlayerPrefs.HasKey("Distance")) {
                if (PlayerPrefs.GetFloat("Distance") < distance) {
                    PlayerPrefs.SetFloat("Distance", distance);
                }
            } else {
                PlayerPrefs.SetFloat("Distance", distance);
            }

            SceneManager.LoadScene("Main");
        }
    }
}
