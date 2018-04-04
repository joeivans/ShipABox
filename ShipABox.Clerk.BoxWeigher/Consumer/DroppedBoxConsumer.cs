using System;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Clerk.BoxWeigher.Consumer
{
    public class DroppedBoxConsumer :
        IConsumer<IClerkReceivedBoxEvent>
    {

        /**
         *  DroppedBoxConsumer
         *
         *  -   Defines how to consume the event's message.
         *
         *  -   Publishes another event when this microservice is done,
         *      signaling the start of next unit of work for the next service.
         */

        public async Task Consume(
            ConsumeContext<IClerkReceivedBoxEvent> context)
        {
            // setup
            var fake = new Faker();
            var boxDimensionsX = Math.Round(fake.Random.Double(5, 20), 2);
            var boxDimensionsY = Math.Round(fake.Random.Double(5, 20), 2);
            var boxDimensionsZ = Math.Round(fake.Random.Double(5, 20), 2);
            var boxWeight = Math.Round(fake.Random.Double(1, 100), 2);
            var boxWeightUnit = "lbs";

            // console out to user
            var strOut = "Clerk weighed box at x:{0},y:{1},z:{2} " +
                         "weight: {3} {4}";
            Console.WriteLine(strOut, boxDimensionsX, boxDimensionsY,
                boxDimensionsZ, boxWeight, boxWeightUnit);

            // publish event
            await context.Publish<IClerkWeighedBoxEvent>(
                new
                {
                    CorrelationId = context.Message.CorrelationId,

                    CustomerName = context.Message.CustomerName,
                    AddressStreet = context.Message.AddressStreet,
                    AddressCity = context.Message.AddressCity,
                    AddressState = context.Message.AddressState,
                    AddressZip = context.Message.AddressZip,

                    BoxDimensionsX = boxDimensionsX,
                    BoxDimensionsY = boxDimensionsY,
                    BoxDimensionsZ = boxDimensionsZ,
                    BoxWeight = boxWeight,
                    BoxWeightUnit = boxWeightUnit
                });

            Console.WriteLine("Press enter to exit");
            Console.Write(">");
        }
    }
}