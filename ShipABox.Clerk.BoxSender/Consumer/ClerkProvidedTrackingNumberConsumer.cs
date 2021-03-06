﻿using System;
using System.Threading.Tasks;
using MassTransit;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Clerk.BoxSender.Consumer
{
    internal class ClerkProvidedTrackingNumberConsumer :
        IConsumer<IClerkProvidedTrackingNumberEvent>
    {
        /**
         *  CustomerPayedInvoiceConsumer
         *
         *  -   Defines how to consume the event's message.
         *
         *  -   Publishes another event when this microservice is done,
         *      signaling the start of next unit of work for the next
         *      service.
         */


        /**
         *  Message template for console.
         */
        private const string StrOut = "Delivery expected by: {0}";


        /**
         *  Required interface implementation.
         */
        public async Task Consume(
            ConsumeContext<IClerkProvidedTrackingNumberEvent> context)
        {
            /**
             *  Setup.
             */
            var timestampBoxLeftFacility = DateTimeOffset.Now;


            /**
             *  Console out to user.
             */
            Console.WriteLine(
                StrOut, context.Message.TimestampDeliveryExpected);


            /**
             *  Publish event.
             */
            await context.Publish<IClerkSentBoxEvent>(
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
                    TrackingNumber = context.Message.TrackingNumber,
                    CarrierId = context.Message.CarrierId,
                    TimestampTrackingCreated =
                        context.Message.TimestampTrackingCreated,
                    TimestampDeliveryExpected =
                        context.Message.TimestampDeliveryExpected,

                    TimestampBoxLeftFacility = timestampBoxLeftFacility
                });


            Console.WriteLine("Press enter to exit");
            Console.Write(">");
        }
    }
}