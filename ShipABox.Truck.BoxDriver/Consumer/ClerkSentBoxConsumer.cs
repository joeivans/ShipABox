using System;
using System.Threading.Tasks;
using MassTransit;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Truck.BoxDriver.Consumer
{
    public class ClerkSentBoxConsumer :
        IConsumer<IClerkSentBoxEvent>
    {

        /**
         *  ClerkSentBoxConsumer
         *
         *  -   Defines how to consume the event's message.
         *
         *  -   Publishes another event when this microservice is done,
         *      signaling the start of next unit of work for the next service.
         */

        public async Task Consume(ConsumeContext<IClerkSentBoxEvent> context)
        {
            // setup
            var driverId = new Guid();
            var truckId = new Guid();
            var timestampDeliverCompleted = DateTimeOffset.Now;

            // console out to user
            var strOut = "Delivery completed: {0}";
            Console.WriteLine(strOut, timestampDeliverCompleted);

            // publish event
            await context.Publish<ITruckDroveBoxEvent>(new
            {
                CorrelationId = context.Message.CorrelationId,
                CustomerName = context.Message.CustomerName,
                AddressStreet = context.Message.AddressStreet,
                AddressCity = context.Message.AddressCity,
                AddressState = context.Message.AddressState,
                AddressZip = context.Message.AddressZip,
                BoxDimensionsX = context.Message.BoxDimensionsX,
                BoxDimensionsY = context.Message.BoxDimensionsY,
                BoxDimensionsZ = context.Message.BoxDimensionsZ,
                BoxWeight = context.Message.BoxWeight,
                BoxWeightUnit = context.Message.BoxWeightUnit,
                SubTotal = context.Message.SubTotal,
                Tax = context.Message.Tax,
                Total = context.Message.Total,
                TimestampInvoiceCreated = context.Message.TimestampInvoiceCreated,
                PaidInFull = context.Message.PaidInFull,
                TimestampPaymentInitiated = context.Message.TimestampPaymentInitiated,
                TrackingNumber = context.Message.TrackingNumber,
                CarrierId = context.Message.CarrierId,
                TimestampTrackingCreated = context.Message.TimestampTrackingCreated,
                TimestampDeliveryExpected = context.Message.TimestampDeliveryExpected,
                TimestampBoxLeftFacility = context.Message.TimestampBoxLeftFacility,

                DriverId  = driverId,
                TruckId = truckId,
                TimestampDeliveryCompleted = timestampDeliverCompleted
            });

            Console.WriteLine("Press enter to exit");
            Console.Write(">");
        }
    }
}
