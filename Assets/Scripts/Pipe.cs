using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        PipeMovement();
    }

    private void PipeMovement() {
        transform.position -= new Vector3(GameManager.gameSpeed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Void") {
            Destroy(this.gameObject);
        } else if (coll.gameObject.tag == "RemovePipe") {
            GameManager.pipes.Remove(this.gameObject);
        }
    }
}
