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
         *  -   Simulates the first step in our workflow, a customer walking a
         *      box in to the shipping store.
         *
         *  -   A new box will be dropped at the store every 5 seconds until
         *      the task is cancelled.
         *
         *  -   A message is sent by using a command in this case because we
         *      only want one endpoint to pick it up. The endpoint here is the
         *      saga's orchestration queue.
         *
         *  -   You'll see in the other microservices, they use events instead
         *      of commands because events can be picked up by many receivers,
         *      like another microservice and the saga orchestrator at the same
         *      time.
         */

        private static void Main(string[] args)
        {
            Console.Title = "(1) Customer.BoxDropper";

            // use a third party faking tool for user input data
            var fake = new Faker();

            // ability to cancel task
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            // send a box every 5 seconds until cancelled
            var boxDropTask = new Task(async () =>
            {
                while (true)
                {
                    var customerName = fake.Name.FullName();
                    var customerAddress = fake.Address.StreetAddress();
                    var customerCity = fake.Address.City();
                    var customerState = fake.Address.State();
                    var customerZip = fake.Address.ZipCode(format: "#####");

                    var strOut =
                        "Customer {0} dropped off a box. It's headed to:\n" +
                        "{1}\n" +
                        "{2}\n" +
                        "{3}\n" +
                        "{4}";

                    Console.WriteLine(strOut, customerName, customerAddress,
                        customerCity, customerState, customerZip);

                    Console.WriteLine("Press enter to exit");
                    Console.Write(">");

                    // this microservice starts the whole saga process.
                    // because it's not subscribed to any events, we don't need
                    // to configure any prefetch count or receive endpoints.
                    var bus = Bus.Factory.CreateUsingRabbitMq(
                        configure: busCfg =>
                        {
                            // configure the host
                            busCfg.Host(
                                hostAddress: new Uri(Constants.Uri),
                                configure: hst =>
                                {
                                    hst.Username(Constants.Username);
                                    hst.Password(Constants.Password);
                                });
                        });

                    // drop off the box and start a new saga
                    var sendToUri = new Uri(
                        $"{Constants.Uri}{Constants.OrchestratorQueue}");
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

                    // wait 5 seconds before sending the next message
                    SpinWait.SpinUntil(
                        condition: () => token.IsCancellationRequested,
                        timeout: TimeSpan.FromSeconds(value: 5));

                    // if we didn't use await in this task, we would use
                    // token.ThrowIfCancellationRequested();
                    // to cancel this task
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, cancellationToken: token);

            // start the task
            boxDropTask.Start();

            // cancel when user hits enter
            Console.ReadKey();
            cts.Cancel();
            Environment.Exit(0);
        }
    }
}