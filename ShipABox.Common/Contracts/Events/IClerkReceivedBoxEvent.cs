using System;

namespace ShipABox.Common.Contracts.Events
{
    public interface IClerkReceivedBoxEvent
    {
        Guid CorrelationId { get; }
        string CustomerName { get; }
        string AddressStreet { get; }
        string AddressCity { get; }
        string AddressState { get; }
        string AddressZip { get; }
    }
}
