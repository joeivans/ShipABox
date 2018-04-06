using System;

namespace ShipABox.Common.Contracts.Events
{
    public interface IClerkProvidedTaxableInvoiceEvent
    {
        /**
         *  A common interface for events where the Clerk provides the
         *  invoice to the Customer.
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
        Guid CorrelationId { get; }
    }
}