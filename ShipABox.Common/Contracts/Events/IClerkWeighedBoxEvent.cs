using System;

namespace ShipABox.Common.Contracts.Events
{
    public interface IClerkWeighedBoxEvent
    {
        /**
         *  A common interface for events where the Clerk weighs the box
         *  and provides the weight to the next microservice that uses
         *  dimensions and weight for its unit of work.
         */


        /**
         *  All the properties required for this event to be useful.
         */
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