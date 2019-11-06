using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeNeural : MonoBehaviour {
    
    private int i_nodes;
    private int h_nodes;
    private int o_nodes;

    Matrix bias_ih;
    Matrix bias_ho;

    public RedeNeural (int i_nodes, int h_nodes, int o_nodes) {
        this.i_nodes = i_nodes;
        this.h_nodes = h_nodes;
        this.o_nodes = o_nodes;

        bias_ih = new Matrix(h_nodes, 1);
        bias_ho = new Matrix(o_nodes, 1);
    }
}
