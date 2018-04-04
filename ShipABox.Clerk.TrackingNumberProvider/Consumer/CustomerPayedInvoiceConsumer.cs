using System;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Clerk.TrackingNumberProvider.Consumer
{
    public class CustomerPayedInvoiceConsumer :
        IConsumer<ICustomerPayedInvoiceEvent>
    {

        /**
         *  CustomerPayedInvoiceConsumer
         *
         *  -   Defines how to consume the event's message.
         *
         *  -   Publishes another event when this microservice is done,
         *      signaling the start of next unit of work for the next service.
         */

        public async Task Consume(
            ConsumeContext<ICustomerPayedInvoiceEvent> context)
        {
            // setup
            var fake = new Faker();
            var trackingNumber = fake.Random.AlphaNumeric(21);
            var carrierId = fake.Random.Word();
            var timestampTrackingCreated = DateTimeOffset.Now;
            var timestampDeliveryExpected =
                DateTimeOffset.Now.Date.AddDays(fake.Random.Int(1, 7));

            // console out to user
            var strOut = "Tracking number: ${0}";
            Console.WriteLine(strOut, trackingNumber);

            // publish event
            await context.Publish<IClerkProvidedTrackingNumberEvent>(
                new
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
                    TimestampInvoiceCreated =
                        context.Message.TimestampInvoiceCreated,
                    PaidInFull = context.Message.PaidInFull,
                    TimestampPaymentInitiated =
                        context.Message.TimestampPaymentInitiated,

                    TrackingNumber = trackingNumber,
                    CarrierId = carrierId,
                    TimestampTrackingCreated = timestampTrackingCreated,
                    TimestampDeliveryExpected = timestampDeliveryExpected
                });

            Console.WriteLine("Press enter to exit");
            Console.Write(">");
        }
    }
}