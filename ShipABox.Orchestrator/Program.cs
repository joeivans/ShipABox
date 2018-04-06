using System;
using Automatonymous;
using MassTransit;
using MassTransit.Saga;
using ShipABox.Common;
using ShipABox.Orchestrator.Service;

namespace ShipABox.Orchestrator
{
    internal class Program
    {
        /**
         *  ShipABox.Orchestrator Saga Orchestrator.
         *
         *  Startup project order: 2
         *
         *  -   Facilitates our Saga.
         */


        private static void Main(string[] args)
        {
            Console.Title = "(2) ShipABox.Orchestrator";


            /**
             *  Create the state machine.
             */
            var machine = new ShipmentSaga();


            /**
             *  The machine needs a repository.
             *
             *  In a real project, use:
             *
             *  new EntityFrameworkSagaRepository<ShipmentSagaState>();
             *
             *  See: http://masstransit-project.com/MassTransit/advanced/sagas/persistence.html
             */
            var repo = new InMemorySagaRepository<ShipmentSagaState>();


            /**
             *  Configure bus to receive DropBox messages on the
             *  orchestration queue.
             */
            var bus = Bus.Factory.CreateUsingRabbitMq(configure: busCfg =>
            {
                var host = busCfg.Host(hostAddress: new Uri(uriString: Constants.Uri), configure: hst =>
                {
                    hst.Username(username: Constants.Username);
                    hst.Password(password: Constants.Password);
                });

                busCfg.PrefetchCount = 8;

                busCfg.ReceiveEndpoint(
                    host: host,
                    queueName: Constants.OrchestratorQueue,
                    configure: endpointConfigurator =>
                    {
                        endpointConfigurator.StateMachineSaga(stateMachine: machine, repository: repo);
                    });
            });


            /**
             *  Start the bus.
             */
            bus.Start();


            Console.WriteLine("Saga is active.");
            Console.WriteLine("Press enter to exit");
            Console.Write(">");
            Console.ReadLine();


            /**
             *  Stop the bus.
             */
            bus.Stop();


            Environment.Exit(exitCode: 0);
        }
    }
}