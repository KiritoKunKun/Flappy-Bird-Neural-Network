using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        Matrix m1 = new Matrix(1, 2);
        Matrix m2 = new Matrix(2, 1);
        Matrix.add(m1, m2);
        Debug.Log(Matrix.multiply(m1, m2).data);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
