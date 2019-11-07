using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Matrix {
    
    public int rows;
    public int cols;
    public double[][] data;

    public Matrix (int rows, int cols) {
        this.rows = rows;
        this.cols = cols;

        this.data = new double[this.rows][];

        for (int i = 0; i < this.rows; i++)  {
            double[] arr = new double[this.cols];

            for (int j = 0; j < this.cols; j++)  {
                arr[j] = Mathf.Floor(UnityEngine.Random.Range(0f, 10f));
            }

            data[i] = new double[arr.Length];
            data[i] = arr;
        }
    }

    public static Matrix arrayToMatrix(double[,] arr) {
        Matrix matrix = new Matrix(arr.Length, 1);

        map(matrix, (num, i, j) => {
            return arr[(int)j, (int) i];
        });

        return matrix;
    }

    public void randomize() {
        map(this, (num, i, j) => {
            return UnityEngine.Random.Range(0f, 1f);
        });
    }

    public static Matrix map(Matrix m, Func<double, double, double, double> func) {
        Matrix matrix = new Matrix(m.rows, m.cols);

        matrix.data = matrix.data.Select((arr, i) => {
            return arr.Select((num, j) => {
                return func(num, i, j);
            }).ToArray();
        }).ToArray();

        return matrix;
    }

    public Matrix map(Func<double, double, double, double> func) {
        this.data = this.data.Select((arr, i) => {
            return arr.Select((num, j) => {
                return func(num, i, j);
            }).ToArray();
        }).ToArray();

        return this;
    }

    public static Matrix add(Matrix a, Matrix b) {
        Matrix matrix = new Matrix(a.rows, b.cols);
        map(matrix, (num, i, j) => {
            return a.data[(int)i][(int)j] + b.data[(int)i][(int)j];
        });

        return matrix;
    }

    public static Matrix multiply (Matrix a, Matrix b) {
        Matrix matrix = new Matrix(a.rows, b.cols);

        map(matrix, (num, i, j) => {
            double sum = 0;
            
            for (int k = 0; k < a.cols; k++) {
                //HERE
                double elm1 = a.data[(int)i][k];
                double elm2 = b.data[k][(int)j];

                sum += elm1 * elm2;
            }

            return sum;
        });

        return matrix;
    }

    public static double sigmoid (float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }
}