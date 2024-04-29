package org.daniil.processors;

import org.apache.flink.api.common.functions.OpenContext;
import org.apache.flink.api.common.functions.RichFilterFunction;
import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.Types;
import org.daniil.models.RocketUpdateFlatModel;

public class FilterDuplicatedEventsFunction extends RichFilterFunction<RocketUpdateFlatModel> {

    //Here key will be order number of the message
    //We are using map state because we simply don't have set api for that)
    private transient MapState<Integer, Boolean> processedMessages;

    @Override
    public void open(OpenContext openContext) throws Exception {
        MapStateDescriptor<Integer, Boolean> mapStateDescriptor = new MapStateDescriptor<>(
                "processed-rocket-updates",
                Types.INT,
                Types.BOOLEAN
        );
        processedMessages = getRuntimeContext().getMapState(mapStateDescriptor);

    }

    @Override
    public boolean filter(RocketUpdateFlatModel rocketUpdateFlatModel) throws Exception {
        var messageNumber = rocketUpdateFlatModel.getMessageNumber();
        if(!processedMessages.contains(messageNumber)){
            processedMessages.put(messageNumber, true);
            return true;
        }
        return false;
    }
}