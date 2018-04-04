using System;
using System.Threading.Tasks;
using MassTransit;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Clerk.InvoiceProvider.Consumer
{
    public class BoxWeighConsumer :
        IConsumer<IClerkWeighedBoxEvent>
    {

        /**
         *  BoxWeighConsumer
         *
         *  -   Defines how to consume the event's message.
         *
         *  -   Publishes another event when this microservice is done,
         *      signaling the start of next unit of work for the next service.
         */

        public async Task Consume(ConsumeContext<IClerkWeighedBoxEvent> context)
        {
            // setup
            var subTotal = Math.Round(context.Message.BoxWeight * .5, 2);
            var tax = Math.Round(0.0875 * subTotal, 2);
            var total = Math.Round((subTotal + tax), 2);
            var timestampInvoiceCreated = DateTimeOffset.Now;

            // console out to user
            var strOut = "Customer owes: ${0}";
            Console.WriteLine(strOut, total);

            // publish event
            await context.Publish<IClerkProvidedTaxableInvoiceEvent>(
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

                    SubTotal = subTotal,
                    Tax = tax,
                    Total = total,
                    TimestampInvoiceCreated = timestampInvoiceCreated
                });

            Console.WriteLine("Press enter to exit");
            Console.Write(">");
        }
    }
}