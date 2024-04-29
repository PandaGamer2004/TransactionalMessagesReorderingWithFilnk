package unit.mocks;

import org.apache.flink.api.common.serialization.DeserializationSchema;
import org.apache.flink.metrics.MetricGroup;
import org.apache.flink.util.UserCodeClassLoader;

public class InitializationContextMock implements DeserializationSchema.InitializationContext {
    @Override
    public MetricGroup getMetricGroup() {
        return null;
    }

    @Override
    public UserCodeClassLoader getUserCodeClassLoader() {
        return null;
    }
}
