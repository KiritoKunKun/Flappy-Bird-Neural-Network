using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    private Player player;
    private BallGenerator ballGenerator;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ballGenerator = GameObject.FindGameObjectWithTag("BallGenerator").GetComponent<BallGenerator>();
    }

    // Update is called once per frame
    void Update() {
        transform.position -= new Vector3(0, 5 * Time.deltaTime, 0);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Void") {
            if (player.losses < 2) {
                player.losses++;
            } else {
                player.losses = 0;
                ballGenerator.spawnPlayer();
            }
        } else if (coll.gameObject.tag == "Player") {
            player.score++;
        }
        
        ballGenerator.spawnBall();
        Destroy(this.gameObject);
    }
}
