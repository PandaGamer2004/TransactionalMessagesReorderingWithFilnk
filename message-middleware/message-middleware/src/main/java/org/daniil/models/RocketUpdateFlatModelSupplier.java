package org.daniil.models;

@FunctionalInterface
public interface RocketUpdateFlatModelSupplier {
    public RocketUpdateFlatModel create(int messageNumber);
}
