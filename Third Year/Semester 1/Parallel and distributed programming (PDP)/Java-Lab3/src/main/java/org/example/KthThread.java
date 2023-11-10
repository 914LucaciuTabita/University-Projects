package org.example;

public class KthThread implements Runnable{
    private Matrix result;
    private Integer taskNumber;
    private Integer numberOfTasks;

    public KthThread(Matrix matrix, Integer taskNumber, Integer numberOfTasks) {
        this.result = matrix;
        this.taskNumber = taskNumber;
        this.numberOfTasks = numberOfTasks;
    }

    @Override
    public void run() {
        int n = Main.n;
        int l = Main.l;
        int m = Main.m;
        int i = 0;
        int j = taskNumber;
        while(true){
            int overflow = j/m;
            i += overflow;
            j -= overflow * m;
            if( i >= n){
                break;
            }
            int sum = 0;
            for(int a = 0; a < l; a++)
            {
                sum += Main.matrix1.getK(i, a) * Main.matrix2.getK(a, j);
            }
            result.insertResult(i, j, sum);
            j+=numberOfTasks;
        }
    }
}
