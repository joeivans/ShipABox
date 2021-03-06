﻿using System;
using MassTransit;
using ShipABox.Common;
using ShipABox.Customer.InvoicePayer.Consumer;

namespace ShipABox.Customer.InvoicePayer
{
    internal class Program
    {
        /**
         *  Customer.InvoicePayer microservice
         *
         *  Startup project order: 5
         *
         *  -   Simulates a customer paying their invoice, based on the
         *      invoice provided as previously determined by another
         *      microservice.
         *
         *  -   This microservice responds to events of type
         *      IClerkProvidedTaxableInvoiceEvent by subscribing a
         *      consumer class of type
         *      ClerkProvidedTaxableInvoiceConsumer. See the consumer
         *      class for the details.
         *
         *  -   The last step in the consumer class's consume method is
         *      to publish an event of type ICustomerPayedInvoiceEvent,
         *      which the next microservice consumes.
         *
         *  -   It's good to remind you that an event can have many
         *      subscribers, therefore many microservices can consume
         *      the event, and the saga orchestrator can also consume it
         *      as well.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(5) Customer.InvoicePayer";

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
                    queueName: Constants.InvoicePayerQueue,
                    configure: cfg =>
                    {
                        cfg.Consumer<ClerkProvidedTaxableInvoiceConsumer>();
                    });
            });


            /**
             *  Start the bus.
             */
            bus.Start();


            Console.WriteLine("Ready to pay invoices.");
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