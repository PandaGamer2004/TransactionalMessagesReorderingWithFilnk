package org.daniil.configuration.models;

import java.time.Duration;

public class FlinkConfiguration {
    private int bufferTimeoutMilliseconds = (int)Duration.ofSeconds(10).toMillis();

    private int maxParallelism = 20;

    private String jobName = "Rocket updates reordered and deduplicator";


    private int checkpointingIntervalMilliseconds = 500;

    public int getBufferTimeoutMilliseconds() {
        return bufferTimeoutMilliseconds;
    }

    public void setBufferTimeoutMilliseconds(int bufferTimeoutMilliseconds) {
        this.bufferTimeoutMilliseconds = bufferTimeoutMilliseconds;
    }

    public int getMaxParallelism() {
        return maxParallelism;
    }

    public void setMaxParallelism(int maxParallelism) {
        this.maxParallelism = maxParallelism;
    }

    public String getJobName() {
        return jobName;
    }

    public void setJobName(String jobName) {
        this.jobName = jobName;
    }

    public int getCheckpointingIntervalMilliseconds() {
        return checkpointingIntervalMilliseconds;
    }

    public void setCheckpointingIntervalMilliseconds(int checkpointingIntervalMilliseconds) {
        this.checkpointingIntervalMilliseconds = checkpointingIntervalMilliseconds;
    }
}
