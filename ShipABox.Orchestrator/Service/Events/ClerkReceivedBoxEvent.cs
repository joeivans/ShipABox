using System;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Orchestrator.Service.Events
{
    public class ClerkReceivedBoxEvent : IClerkReceivedBoxEvent
    {
        /**
         *  The event that the Saga publishes to a microservice to start
         *  the saga after a box has been received.
         *
         *  Note: This class could have had a more restrictive access
         *  modifier since the only consumer is the ShipmentSaga class.
         */


        /**
         *  Keep the Saga's state for call forwarding.
         *
         *  Injected by the constructor.
         */
        private readonly ShipmentSagaState _shipmentSagaState;


        public ClerkReceivedBoxEvent(ShipmentSagaState shipmentSagaState)
        {
            _shipmentSagaState = shipmentSagaState;
        }


        /**
         *  Forward the calls to the Saga state to make things visually
         *  cleaner.
         *
         *  NOTE: These could have been assigned individually in the
         *  constructor.
         */
        public string CustomerName => _shipmentSagaState.CustomerName;
        public string AddressStreet => _shipmentSagaState.AddressStreet;
        public string AddressCity => _shipmentSagaState.AddressCity;
        public string AddressState => _shipmentSagaState.AddressState;
        public string AddressZip => _shipmentSagaState.AddressZip;
        public Guid CorrelationId => _shipmentSagaState.CorrelationId;
    }
}