﻿using System;
using MassTransit;
using ShipABox.Clerk.InvoiceProvider.Consumer;
using ShipABox.Common;

namespace ShipABox.Clerk.InvoiceProvider
{
    internal class Program
    {
        /**
         *  Clerk.InvoiceProvider microservice
         *
         *  Startup project order: 4
         *
         *  -   Simulates a clerk providing an invoice to a customer,
         *      based on the weight of the box as previously determined
         *      by another microservice.
         *
         *  -   This microservice responds to events of type
         *      IClerkWeighedBoxEvent by subscribing a consumer class of
         *      type BoxWeighConsumer. See the consumer class for the
         *      details.
         *
         *  -   The last step in the consumer class's consume method is
         *      to publish an event of type IClerkWeighedBoxEvent, which
         *      the next microservice consumes.
         *
         *  -   It's good to remind you that an event can have many
         *      subscribers, therefore many microservices can consume
         *      the event, and the saga orchestrator can also consume it
         *      as well.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(4) Clerk.InvoiceProvider";


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
                    queueName: Constants.InvoiceProviderQueue,
                    configure: cfg =>
                    {
                        cfg.Consumer<BoxWeighConsumer>();
                    });
            });


            /**
             *  Start the bus.
             */
            bus.Start();


            Console.WriteLine("Ready to provide invoices.");
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