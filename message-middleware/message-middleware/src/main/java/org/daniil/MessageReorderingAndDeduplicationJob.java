package org.daniil;


import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.connector.base.DeliveryGuarantee;
import org.apache.flink.connector.kafka.sink.KafkaRecordSerializationSchema;
import org.apache.flink.connector.kafka.sink.KafkaSink;
import org.apache.flink.connector.kafka.source.KafkaSource;
import org.apache.flink.connector.kafka.source.enumerator.initializer.OffsetsInitializer;
import org.apache.flink.formats.json.JsonDeserializationSchema;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.CheckpointConfig;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.daniil.configuration.models.FlinkConfiguration;
import org.daniil.configuration.models.RocketUpdatesKafkaConfiguration;
import org.daniil.models.BatchedRocketUpdateModel;
import org.daniil.models.RocketUpdateModel;
import org.daniil.processors.RocketUpdateModelsProcessor;


public class MessageReorderingAndDeduplicationJob {

	public static void main(String[] args) throws Exception {
		final StreamExecutionEnvironment executionEnvironment
				= getConfiguredExecutionEnvironment();


		final var kafkaConfig = new RocketUpdatesKafkaConfiguration();
		KafkaSource<RocketUpdateModel> rocketUpdateModelSource
				= getKafkaSourceForUpdateModels(
						kafkaConfig
		);


		KafkaSink<BatchedRocketUpdateModel> batchedRocketUpdateModelsSync
				= getKafkaSink(kafkaConfig);


		DataStream<RocketUpdateModel> rocketUpdateModelStream = executionEnvironment.fromSource(
				rocketUpdateModelSource,
				WatermarkStrategy.noWatermarks(),
				kafkaConfig.getSourceName()
				);


		var rocketModelsProcessor = new RocketUpdateModelsProcessor();
		var outboundStream = rocketModelsProcessor
				.registerProcessing(rocketUpdateModelStream);

		outboundStream.sinkTo(
				batchedRocketUpdateModelsSync
		);

		executionEnvironment.execute(
				kafkaConfig.getJobName()
		);

	}


	private static StreamExecutionEnvironment getConfiguredExecutionEnvironment(){
		//As of now leave as constants in future could be extended to dynamically discoverable
		var flinkConfiguration = new FlinkConfiguration();

		var environment = StreamExecutionEnvironment.getExecutionEnvironment();
		environment.setMaxParallelism(
				flinkConfiguration.getMaxParallelism()
		);
		environment.setBufferTimeout(
				flinkConfiguration.getBufferTimeoutMilliseconds()
		);

		environment.enableCheckpointing(
				flinkConfiguration.getCheckpointingIntervalMilliseconds()
		);
		CheckpointConfig config = environment.getCheckpointConfig();
		config.setCheckpointingMode(CheckpointingMode.EXACTLY_ONCE);

		return environment;
	}


	private static KafkaSink<BatchedRocketUpdateModel> getKafkaSink(RocketUpdatesKafkaConfiguration configuration){
		var serializationSchema
				= BatchedRocketUpdateModel.getSerializationSchema();
		return KafkaSink.<BatchedRocketUpdateModel>builder()
				.setBootstrapServers(configuration.getBootstrapServer())
				.setRecordSerializer(
						KafkaRecordSerializationSchema
								.builder()
								.setValueSerializationSchema(serializationSchema)
								.setTopic(configuration.getOutputTopic())
								.build()
				)
				.setDeliveryGuarantee(DeliveryGuarantee.AT_LEAST_ONCE)
				.build();
	}
	private static KafkaSource<RocketUpdateModel> getKafkaSourceForUpdateModels(
			RocketUpdatesKafkaConfiguration configuration
	) {
		JsonDeserializationSchema<RocketUpdateModel> deserializationSchema
				= RocketUpdateModel.getSchema();
		KafkaSource<RocketUpdateModel> rocketUpdatesSource
				= KafkaSource.<RocketUpdateModel>builder()
				.setBootstrapServers(configuration.getBootstrapServer())
				.setTopics(configuration.getInputTopic())
				.setGroupId(configuration.getConsumerGroupId())
				.setStartingOffsets(OffsetsInitializer.earliest())
				.setValueOnlyDeserializer(deserializationSchema)
				.build();

		return rocketUpdatesSource;
	}



}
