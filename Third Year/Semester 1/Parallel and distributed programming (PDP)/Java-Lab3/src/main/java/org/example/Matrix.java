package org.example;

public class Matrix {
    private Integer[][] matrix;
    private Integer n, m;

    public Matrix(Integer n, Integer m) {
        this.n = n;
        this.m = m;
        matrix = new Integer[n][m];
    }

    public Integer[][] getMatrix() {
        return matrix;
    }

    public Integer getN() {
        return n;
    }

    public Integer getM() {
        return m;
    }

    public Integer getK(Integer n, Integer m){
        return matrix[n][m];
    }

    public void insertResult(Integer n, Integer m, Integer result){
        this.matrix[n][m] = result;
    }

    public String print() {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                stringBuilder.append(matrix[i][j]+ " ");
            }
            stringBuilder.append("\n");
        }
        stringBuilder.append("\n");
        return stringBuilder.toString();
    }
}

