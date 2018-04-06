using System;

namespace ShipABox.Common.Contracts.Events
{
    public interface IClerkReceivedBoxEvent
    {
        /**
         *  A common interface for events where the Clerk acknowledges
         *  receipt of the box from the Customer.
         */


        /**
         *  All the properties required for this event to be useful.
         */
        Guid CorrelationId { get; }
        string CustomerName { get; }
        string AddressStreet { get; }
        string AddressCity { get; }
        string AddressState { get; }
        string AddressZip { get; }
    }
}