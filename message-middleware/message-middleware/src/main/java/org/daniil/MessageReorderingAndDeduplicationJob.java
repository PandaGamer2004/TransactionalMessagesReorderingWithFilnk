package org.daniil;

import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

public class MessageReorderingAndDeduplicationJob {

	public static void main(String[] args) throws Exception {
		final StreamExecutionEnvironment env
				= StreamExecutionEnvironment.getExecutionEnvironment();


		env.execute("Flink is exeucted");
	}
}
