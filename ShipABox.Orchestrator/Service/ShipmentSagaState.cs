using System;
using Automatonymous;

namespace ShipABox.Orchestrator.Service
{
    public class ShipmentSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string State { get; set; }

        public string CustomerName { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }

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
    }
}