using System;
using Automatonymous;

namespace ShipABox.Orchestrator.Service
{
    public class ShipmentSagaState : SagaStateMachineInstance
    {
        /**
         *  This is the state of the saga.
         *
         *  Each saga in progress gets an instance of this class and
         *  that is practically the only thing unique to it.
         *
         *  You can initialize these properties with data from the
         *  events it's subscribed to which allows the state machine to
         *  not only switch states as before, but to be more informed
         *  about the entire saga.
         */


        /**
         *  CorrelationId is a required interface implementation.
         */
        public Guid CorrelationId { get; set; }


        /**
         *  State is used to track the state of the saga, in this case,
         *  it's in string form.
         */
        public string State { get; set; }


        /**
         *  These properties will be assigned by the events' data
         *  received.
         */
        public string CustomerName { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }


        /**
         *  You can add as many properties as you like to track data.
         *
         *  Such as these:

        public double BoxDimensionsX { get; set; }
        public double BoxDimensionsY { get; set; }
        public double BoxDimensionsZ { get; set; }

        public double BoxWeight { get; set; }
        public string BoxWeightUnit { get; set; }

        public double SubTotal { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public DateTimeOffset TimestampInvoiceCreated { get; set; }

        public bool PaidInFull { get; set; }
        public DateTimeOffset TimestampPaymentInitiated { get; set; }

        public string TrackingNumber { get; set; }
        public string CarrierId { get; set; }
        public DateTimeOffset TimestampTrackingCreated { get; set; }
        public DateTimeOffset TimestampDeliveryExpected { get; set; }
        public DateTimeOffset TimestampDeliveryCompleted { get; set; }

        public DateTimeOffset TimestampBoxLeftFacility { get; set; }

        public Guid DriverId { get; set; }
        public Guid TruckId { get; set; }

        */
    }
}