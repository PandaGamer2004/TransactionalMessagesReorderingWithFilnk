package org.daniil;

import org.apache.flink.connector.kafka.source.KafkaSource;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

public class MessageReorderingAndDeduplicationJob {

	public static void main(String[] args) throws Exception {

		KafkaSource<String> source = KafkaSource.<String>builder()
				.setBootstrapServers("localhost:9092")
				.setTopics("")
		final StreamExecutionEnvironment env
				= StreamExecutionEnvironment.getExecutionEnvironment();


		env.execute("Flink is exeucted");
	}
}
