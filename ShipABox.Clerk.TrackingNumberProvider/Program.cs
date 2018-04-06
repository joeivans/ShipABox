using System;
using MassTransit;
using ShipABox.Clerk.TrackingNumberProvider.Consumer;
using ShipABox.Common;

namespace ShipABox.Clerk.TrackingNumberProvider
{
    internal class Program
    {
        /**
         *  Clerk.TrackingNumberProvider microservice
         *
         *  Startup project order: 6
         *
         *  -   Simulates a clerk providing a tracking number, based on
         *      the customer's payment being completed as previously
         *      determined by another microservice.
         *
         *  -   This microservice responds to events of type
         *      ICustomerPayedInvoiceEvent by subscribing a consumer
         *      class of type CustomerPayedInvoiceConsumer. See the
         *      consumer class for the details.
         *
         *  -   The last step in the consumer class's consume method is
         *      to publish an event of type
         *      IClerkProvidedTrackingNumberEvent, which the next
         *      microservice consumes.
         *
         *  -   It's good to remind you that an event can have many
         *      subscribers, therefore many microservices can consume
         *      the event, and the saga orchestrator can also consume it
         *      as well.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(6) Clerk.TrackingNumberProvider";


            /**
             *  Get messages on this microservice's queue.
             */
            var bus = Bus.Factory.CreateUsingRabbitMq(configure: busCfg =>
            {
                /**
                 *  Configure the host.
                 */
                var host = busCfg.Host(
                    hostAddress: new Uri(Constants.Uri),
                    configure: cfg =>
                    {
                        cfg.Username(Constants.Username);
                        cfg.Password(Constants.Password);
                    });


                /**
                 *  Get up to 8 messages at a time.
                 */
                busCfg.PrefetchCount = 8;


                /**
                 *  Subscribe a consumer to the endpoint.
                 */
                busCfg.ReceiveEndpoint(
                    host: host,
                    queueName: Constants.TrackingNumberProviderQueue,
                    configure: cfg =>
                    {
                        cfg.Consumer<CustomerPayedInvoiceConsumer>();
                    });
            });


            /**
             *  Start the bus.
             */
            bus.Start();


            Console.WriteLine("Ready to provide tracking numbers.");
            Console.WriteLine("Press enter to exit");
            Console.Write(">");
            Console.ReadLine();


            /**
             *  Stop the bus.
             */
            bus.Stop();


            Environment.Exit(0);
        }
    }
}