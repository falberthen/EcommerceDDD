﻿namespace EcommerceDDD.ShipmentProcessing.Domain;

public sealed class ShipmentId : StronglyTypedId<Guid>
{
    public static ShipmentId Of(Guid value)
    {
        return new ShipmentId(value);
    }

    public ShipmentId(Guid value) : base(value)
    {
    }
}
