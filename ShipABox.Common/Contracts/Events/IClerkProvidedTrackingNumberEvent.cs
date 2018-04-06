using System;

namespace ShipABox.Common.Contracts.Events
{
    public interface IClerkProvidedTrackingNumberEvent
    {
        /**
         *  A common interface for events where the Clerk provides the
         *  tracking number to the customer.
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
        double SubTotal { get; }
        double Tax { get; }
        double Total { get; }
        DateTimeOffset TimestampInvoiceCreated { get; }
        bool PaidInFull { get; }
        DateTimeOffset TimestampPaymentInitiated { get; }

        string TrackingNumber { get; }
        string CarrierId { get; }
        DateTimeOffset TimestampTrackingCreated { get; }
        DateTimeOffset TimestampDeliveryExpected { get; }
        Guid CorrelationId { get; }
    }
}