using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        RedeNeural nn = new RedeNeural(1, 3, 1);
        double[,] arr = new double[1, 2];
        nn.feedFoward(arr);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
