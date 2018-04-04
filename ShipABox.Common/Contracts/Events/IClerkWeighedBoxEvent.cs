using System;

namespace ShipABox.Common.Contracts.Events
{
    public interface IClerkWeighedBoxEvent
    {
        string CustomerName { get; }
        string AddressStreet { get; }
        string AddressCity { get; }
        string AddressState { get; }
        string AddressZip { get; }

        double BoxDimensionsX { get; }
        double BoxDimensionsY { get; }
        double BoxDimensionsZ { get; }
        double BoxWeight { get; }
        string BoxWeightUnit { get; }
        Guid CorrelationId { get; }
    }
}
