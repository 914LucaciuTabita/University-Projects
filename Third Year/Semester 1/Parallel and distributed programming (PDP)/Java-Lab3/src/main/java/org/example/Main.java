package org.example;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.Executors;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

public class Main {
    public static int n=100;
    public static int l=100;
    public static int m=100;
    public static int numberOfTasks=10;
    public static Matrix matrix1;
    public static Matrix matrix2;
    public static Matrix resultMatrix;

    public static void generateMatrices() throws IOException {
        Random random = new Random();
        matrix1 = new Matrix(n,l);
        matrix2 = new Matrix(l,m);
        resultMatrix = new Matrix(n,m);
        for(int i = 0; i < n; i++ ){
            for(int j = 0; j < l; j++ ){
                matrix1.insertResult(i,j,random.nextInt(8)+1);
            }
        }
        for(int i = 0; i < l; i++ ){
            for(int j = 0; j < m; j++ ) {
                matrix2.insertResult(i, j, random.nextInt(8) + 1);
            }
        }
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                int sum = 0;
                for (int k = 0; k < l; k++) {
                    sum += matrix1.getK(i,k) * matrix2.getK(k,j);
                }
                resultMatrix.insertResult(i,j,sum);
            }
        }
    }

    public static String productByThreads() throws InterruptedException, IOException {
        Matrix result = new Matrix(n,m);
        List<Thread> threadList = new ArrayList<>();
        for(int i=0; i < numberOfTasks; i++){
            threadList.add(new Thread(new KthThread(result,i,numberOfTasks)));
        }
        final long startTime = System.currentTimeMillis();
        for (Thread thread : threadList) {
            thread.start();
        }
        for (Thread thread : threadList) {
            thread.join();
        }
        final long stopTime = System.currentTimeMillis();
        return "Real time for classic thread execution = " + (stopTime-startTime) + "ms\n\n";
    }

    public static String productByPool() throws InterruptedException, IOException {
        ThreadPoolExecutor executor = (ThreadPoolExecutor) Executors.newFixedThreadPool(numberOfTasks/2);
        Matrix result = new Matrix(n,m);
        List<Runnable> tasks = new ArrayList<>();
        for (int i = 0; i < numberOfTasks; i++) {
            tasks.add(new KthThread(result, i, numberOfTasks));
        }
        final long startTime = System.currentTimeMillis();
        for (Runnable task : tasks) {
            executor.execute(task);
        }
        executor.shutdown();
        while (!executor.awaitTermination(1, TimeUnit.DAYS)) {
            System.out.println("Not yet. Still waiting for termination");
        }
        final long stopTime = System.currentTimeMillis();
        return "Real time for thread pool execution = " + (stopTime-startTime) + "ms\n\n";
    }

    public static void main(String[] args) throws IOException, InterruptedException {
        n = 100;
        l = 100;
        m = 100;
        numberOfTasks = 10;
        generateMatrices();
        File output = Paths.get("src","main","resources","output.txt").toFile();
        output.createNewFile();
        FileWriter fileWriter = new FileWriter(output);
        fileWriter.write("Trying classic strategy ... \nInput parameters: \nmatrix 1 is of size "+ n + " and " +l+"\n");
        fileWriter.write("matrix 2 is of size "+l + " and "+ m +"\n");
        fileWriter.write("number of tasks is " + numberOfTasks+ "\n");
        fileWriter.write( productByThreads());
        fileWriter.write("Trying thread pool strategy ... \nInput parameters: \nmatrix 1 is of size "+ n + " and " +l+"\n");
        fileWriter.write("matrix 2 is of size "+l + " and "+ m +"\n");
        fileWriter.write("number of tasks is " + numberOfTasks+ "\n");
        fileWriter.write( productByPool());

        n = 800;
        l = 1000;
        m = 900;
        numberOfTasks = 100;
        matrix1 = null;
        matrix2 = null;
        resultMatrix = null;
        generateMatrices();
        fileWriter.write("Trying classic strategy ... \nInput parameters: \nmatrix 1 is of size "+ n + " and " +l+"\n");
        fileWriter.write("matrix 2 is of size "+l + " and "+ m +"\n");
        fileWriter.write("number of tasks is " + numberOfTasks+ "\n");
        fileWriter.write( productByThreads());
        fileWriter.write("Trying thread pool strategy ... \nInput parameters: \nmatrix 1 is of size "+ n + " and " +l+"\n");
        fileWriter.write("matrix 2 is of size "+l + " and "+ m +"\n");
        fileWriter.write("number of tasks is " + numberOfTasks+ "\n");
        fileWriter.write( productByPool());

        n = 800;
        l = 1000;
        m = 900;
        numberOfTasks = 32;
        matrix1 = null;
        matrix2 = null;
        resultMatrix = null;
        generateMatrices();
        fileWriter.write("Trying classic strategy ... \nInput parameters: \nmatrix 1 is of size "+ n + " and " +l+"\n");
        fileWriter.write("matrix 2 is of size "+l + " and "+ m +"\n");
        fileWriter.write("number of tasks is " + numberOfTasks+ "\n");
        fileWriter.write( productByThreads());
        fileWriter.write("Trying thread pool strategy ... \nInput parameters: \nmatrix 1 is of size "+ n + " and " +l+"\n");
        fileWriter.write("matrix 2 is of size "+l + " and "+ m +"\n");
        fileWriter.write("number of tasks is " + numberOfTasks+ "\n");
        fileWriter.write( productByPool());
        fileWriter.close();
    }
}