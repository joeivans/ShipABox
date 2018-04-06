using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using ShipABox.Common;
using ShipABox.Common.Contracts.Commands;

namespace ShipABox.Customer.BoxDropper
{
    internal class Program
    {
        /**
         *  Customer.BoxDropper microservice
         *
         *  Startup project order: 1
         *
         *  -   Simulates the first step in our workflow, a customer
         *      walking a box in to the shipping store.
         *
         *  -   A new box will be dropped at the store every 5 seconds
         *      until the task is cancelled.
         *
         *  -   A message is sent by using a command in this case
         *      because we only want one endpoint to pick it up. The
         *      endpoint here is the saga's orchestration queue.
         *
         *  -   You'll see in the other microservices, they use events
         *      instead of commands because events can be picked up by
         *      many receivers, like another microservice and the saga
         *      orchestrator at the same time.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(1) Customer.BoxDropper";


            /**
             *  Use a third party faking tool for user input data.
             */
            var fake = new Faker();


            /**
             *  Ability to cancel the Task.
             */
            var cts = new CancellationTokenSource();
            var token = cts.Token;


            /**
             *  Configure the message bus.
             */
            var bus = Bus.Factory.CreateUsingRabbitMq(
                configure: busCfg =>
                {
                    /**
                     *  Configure the host.
                     */
                    busCfg.Host(
                        hostAddress: new Uri(Constants.Uri),
                        configure: hst =>
                        {
                            hst.Username(Constants.Username);
                            hst.Password(Constants.Password);
                        });
                });


            /**
             *  We need the URI to send the messages to.
             */
            var sendToUri = new Uri(
                $"{Constants.Uri}{Constants.OrchestratorQueue}");


            /**
             *  Progress message template for the console.
             */
            const string strOut =
                "Customer {0} dropped off a box. It's headed to:\n" +
                "{1}\n" +
                "{2}\n" +
                "{3}\n" +
                "{4}";


            /**
             *  Send a box every 5 seconds until cancelled
             */
            var boxDropTask = new Task(async () =>
            {
                while (true)
                {
                    /**
                     *  Fake some data.
                     */
                    var customerName = fake.Name.FullName();
                    var customerAddress = fake.Address.StreetAddress();
                    var customerCity = fake.Address.City();
                    var customerState = fake.Address.State();
                    var customerZip = fake.Address.ZipCode(format: "#####");

                    Console.WriteLine(strOut, customerName, customerAddress,
                        customerCity, customerState, customerZip);

                    Console.WriteLine("Press enter to exit");
                    Console.Write(">");


                    /**
                     *  Drop off the box and start a new saga.
                     */
                    var endPoint = await bus.GetSendEndpoint(sendToUri);

                    await endPoint.Send<ICustomerDroppedBoxCommand>(
                        new
                        {
                            CustomerName = customerName,
                            AddressStreet = customerAddress,
                            AddressCity = customerCity,
                            AddressState = customerState,
                            AddressZip = customerZip
                        });


                    /**
                     *  Wait 5 seconds before sending the next message.
                     */
                    SpinWait.SpinUntil(
                        condition: () => token.IsCancellationRequested,
                        timeout: TimeSpan.FromSeconds(value: 5));


                    /**
                     *  If we didn't use await in this task, we would
                     *  use token.ThrowIfCancellationRequested(); to
                     *  cancel this task. They just don't play nice
                     *  together.
                     */
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, cancellationToken: token);


            /**
             *  Start the task.
             */
            boxDropTask.Start();


            /**
             *  Cancel when user hits enter.
             */
            Console.ReadKey();
            cts.Cancel();


            Environment.Exit(0);
        }
    }
}