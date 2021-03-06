﻿using System;
using MassTransit;
using ShipABox.Common;
using ShipABox.Truck.BoxDriver.Consumer;

namespace ShipABox.Truck.BoxDriver
{
    internal class Program
    {
        /**
         *  Truck.BoxDriver microservice
         *
         *  Startup project order: 8
         *
         *  -   Simulates a truck driving a box to its destination,
         *      based on the customer's payment being completed as
         *      previously determined by another microservice.
         *
         *  -   This microservice responds to events of type
         *      IClerkSentBoxEvent by subscribing a consumer class of
         *      type ClerkSentBoxConsumer. See the consumer class for
         *      the details.
         *
         *  -   The last step in the consumer class's consume method is
         *      to publish an event of type ITruckDroveBoxEvent, which
         *      the next microservice consumes.
         *
         *  -   It's good to remind you that an event can have many
         *      subscribers, therefore many microservices can consume
         *      the event, and the saga orchestrator can also consume it
         *      as well.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(8) Truck.BoxDriver";

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
                    configure: hst =>
                    {
                        hst.Username(Constants.Username);
                        hst.Password(Constants.Password);
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
                    queueName: Constants.BoxDriverQueue,
                    configure: cfg =>
                    {
                        cfg.Consumer<ClerkSentBoxConsumer>();
                    });
            });


            /**
             *  Start the bus.
             */
            bus.Start();


            Console.WriteLine("Ready to deliver boxes.");
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