using System;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Orchestrator.Service.Events
{
    public class ClerkReceivedBoxEvent : IClerkReceivedBoxEvent
    {
        private readonly ShipmentSagaState _shipmentSagaState;

        public ClerkReceivedBoxEvent(ShipmentSagaState shipmentSagaState)
        {
            _shipmentSagaState = shipmentSagaState;
        }

        public string CustomerName => _shipmentSagaState.CustomerName;
        public string AddressStreet => _shipmentSagaState.AddressStreet;
        public string AddressCity => _shipmentSagaState.AddressCity;
        public string AddressState => _shipmentSagaState.AddressState;
        public string AddressZip => _shipmentSagaState.AddressZip;
        public Guid CorrelationId => _shipmentSagaState.CorrelationId;
    }
}
