package org.daniil.models;

public interface Mapper<TInbound, TOutbound>{
    TOutbound map(TInbound inbound);
}
