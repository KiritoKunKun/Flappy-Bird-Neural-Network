using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeNeural {
    
    private int i_nodes;
    private int h_nodes;
    private int o_nodes;

    Matrix bias_ih;
    Matrix bias_ho;

    Matrix weights_ih;
    Matrix weights_ho;

    public RedeNeural (int i_nodes, int h_nodes, int o_nodes) {
        this.i_nodes = i_nodes;
        this.h_nodes = h_nodes;
        this.o_nodes = o_nodes;

        bias_ih = new Matrix(h_nodes, 1);
        bias_ih.randomize();
        
        bias_ho = new Matrix(o_nodes, 1);
        bias_ho.randomize();

        weights_ih = new Matrix(this.h_nodes, this.i_nodes);
        weights_ih.randomize();

        weights_ho = new Matrix(this.o_nodes, this.h_nodes);
        weights_ho.randomize();
    }

    public void feedFoward(double[,] input) {
        //INPUT -> HIDDEN
        Matrix inp = Matrix.arrayToMatrix(input);

        Matrix hidden = Matrix.multiply(this.weights_ih, inp);
        hidden = Matrix.add(hidden, this.bias_ih);

        hidden.map((num, i, j) => {
            return Matrix.sigmoid((float)hidden.data[(int)i][(int)j]);
        });

        //HIDDEN -> OUTPUT
        Matrix output = Matrix.multiply(this.weights_ho, hidden);
        output = Matrix.add(output, this.bias_ho);
        
        output.map((num, i, j) => {
            return Matrix.sigmoid((float)output.data[(int)i][(int)j]);
        });

        Debug.Log(output.data[0][0]);
    }
}
