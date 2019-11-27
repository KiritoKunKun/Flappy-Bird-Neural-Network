using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeNeural {
    
    private int i_nodes;
    private int h_nodes;
    private int o_nodes;
    private double learning_rate;

    public Matrix bias_ih;
    public Matrix bias_ho;

    public Matrix weights_ih;
    public Matrix weights_ho;

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

        learning_rate = 0.1d;
    }

    public static double sigmoid (double x) {
        return 1 / (1 + Math.Exp(-x));
    }

    public static double dsigmoid (double x) {
        return x * (1 - x);
    }

    public void train(double[] arr, double[] target) {
        //INPUT -> HIDDEN
        Matrix input = Matrix.arrayToMatrix(arr);
        Matrix hidden = Matrix.multiply(weights_ih, input);
        hidden = Matrix.add(hidden, bias_ih);
        hidden.map((num, i, j) => {
            return sigmoid(hidden.data[(int)i][(int)j]);
        });
        
        //HIDDEN -> OUTPUT
        Matrix output = Matrix.multiply(weights_ho, hidden);
        output = Matrix.add(output, bias_ho);
        output.map((num, i, j) => {
            return sigmoid(output.data[(int)i][(int)j]);
        });

        //Debug.Log(output.data[0][0]);

        //BACKPROPAGATION

        //OUTPUT -> HIDDEN
        Matrix expected = Matrix.arrayToMatrix(target);
        Matrix output_error = Matrix.subtract(expected, output);
        Matrix d_output = Matrix.map(output, (num, i, j) => {
            return dsigmoid(output.data[(int)i][(int)j]);
        });

        //CORRECOES DOS PESOS

        Matrix gradient = Matrix.hadamard(d_output, output_error);
        gradient = Matrix.escalarMultiply(gradient, learning_rate);

        //Ajust Bias O -> H
        bias_ho = Matrix.add(bias_ho, gradient);

		//Ajust Weights O -> H
		Matrix hidden_T = Matrix.transpose(hidden);
		Matrix weights_ho_deltas = Matrix.multiply(gradient, hidden_T);
        weights_ho = Matrix.add(weights_ho, weights_ho_deltas);

        //HIDDEN -> INPUT
        Matrix weights_ho_T = Matrix.transpose(weights_ho);
        Matrix hidden_error = Matrix.multiply(weights_ho_T, output_error);
        Matrix d_hidden = Matrix.map(hidden, (num, i, j) => {
            return dsigmoid(hidden.data[(int)i][(int)j]);
        });
        Matrix input_T = Matrix.transpose(input);

        Matrix gradient_H = Matrix.hadamard(hidden_error, d_hidden);
        gradient_H = Matrix.escalarMultiply(gradient_H, learning_rate);

        //Ajust Bias O -> H
        bias_ih = Matrix.add(bias_ih, gradient_H);

        //Ajust Bias H -> I
        Matrix weights_ih_deltas = Matrix.multiply(gradient_H, input_T);
        weights_ih = Matrix.add(weights_ih, weights_ih_deltas);
    }

    //Calculates output and return a output value
    public double[] predict(double[] arr) {
        //INPUT -> HIDDEN
        Matrix input = Matrix.arrayToMatrix(arr);

        Matrix hidden = Matrix.multiply(weights_ih, input);
        hidden = Matrix.add(hidden, bias_ih);
        hidden.map((num, i, j) => {
            return sigmoid(hidden.data[(int)i][(int)j]);
        });

        //HIDDEN -> OUTPUT
        Matrix output = Matrix.multiply(weights_ho, hidden);
        output = Matrix.add(output, bias_ho);
        output.map((num, i, j) => {
            return sigmoid(output.data[(int)i][(int)j]);
        });

        double[] output_arr = Matrix.matrixToArray(output);
        return output_arr;
    }
}
