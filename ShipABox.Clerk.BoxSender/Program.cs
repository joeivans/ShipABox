﻿using System;
using MassTransit;
using ShipABox.Clerk.BoxSender.Consumer;
using ShipABox.Common;

namespace ShipABox.Clerk.BoxSender
{
    internal class Program
    {
        /**
         *  Clerk.BoxSender microservice
         *
         *  Startup project order: 7
         *
         *  -   Simulates a clerk moving a box from the counter top to
         *      the back of the shipping truck, based on the customer's
         *      payment being completed as previously determined by
         *      another microservice.
         *
         *  -   This microservice responds to events of type
         *      IClerkProvidedTrackingNumberEvent by subscribing a
         *      consumer class of type
         *      ClerkProvidedTrackingNumberConsumer. See the consumer
         *      class for the details.
         *
         *  -   The last step in the consumer class's consume method is
         *      to publish an event of type IClerkSentBoxEvent, which
         *      the next microservice consumes.
         *
         *  -   It's good to remind you that an event can have many
         *      subscribers, therefore many microservices can consume
         *      the event, and the saga orchestrator can also consume it
         *      as well.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(7) Clerk.BoxSender";


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
                    queueName: Constants.BoxSenderQueue,
                    configure: cfg =>
                    {
                        cfg.Consumer<ClerkProvidedTrackingNumberConsumer>();
                    });
            });


            /**
             *  Start the bus.
             */
            bus.Start();


            Console.WriteLine("Ready to send boxes.");
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