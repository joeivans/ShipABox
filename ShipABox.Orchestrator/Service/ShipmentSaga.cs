using System;
using Automatonymous;
using ShipABox.Common.Contracts.Commands;
using ShipABox.Common.Contracts.Events;
using ShipABox.Orchestrator.Service.Events;

namespace ShipABox.Orchestrator.Service
{
    // Seal the saga class because of virtual calls in the constructor
    public sealed class ShipmentSaga : MassTransitStateMachine<ShipmentSagaState>
    {
        /**
         *  This class is where the orchestration happens.
         */

        // Declare the possible sates this machine can be in
        public State BoxInStore { get; set; }
        public State BoxInTruck { get; set; }
        public State BoxDelivered { get; set; }

        // Declare the events this saga subscribes to.
        // The saga doesn't need to react to every event, FYI.
        public Event<ICustomerDroppedBoxCommand> CustomerDroppedOffBox { get; private set; }
        public Event<IClerkWeighedBoxEvent> ClerkWeighedBox { get; private set; }
        public Event<IClerkProvidedTaxableInvoiceEvent> ClerkProvidedTaxableInvoice { get; private set; }
        public Event<ICustomerPayedInvoiceEvent> CustomerPayedInvoice { get; private set; }
        public Event<IClerkProvidedTrackingNumberEvent> ClerkProvidedTrackingNumber { get; private set; }
        public Event<IClerkSentBoxEvent> ClerkSentBox { get; private set; }

        // Events that will cause the saga to end
        public Event<ICustomerCancelledInvoiceEvent> CustomerCancelledInvoice { get; private set; }
        public Event<ITruckDroveBoxEvent> TruckDroveBox { get; private set; }

        // The constructor is where all the magic happens
        public ShipmentSaga()
        {
            // Set the state property, allowing the machine to track the state
            InstanceState(s => s.State);

            // Wire up the events for this saga.
            // For the first correlation, use two fields that match in the saga
            //  state and the first message type. Then you have to tell the saga
            //  how to set its own Correlation Id. In this case we're giving it
            //  a new Guid.
            Event(() => CustomerDroppedOffBox,
                configureEventCorrelation: configurator =>
                    configurator.CorrelateBy(
                            state => state.CustomerName,
                            context => context.Message.CustomerName)
                        .SelectId(context => Guid.NewGuid()));

            Event(() => ClerkWeighedBox,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            Event(() => ClerkProvidedTaxableInvoice,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            Event(() => CustomerPayedInvoice,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            Event(() => CustomerCancelledInvoice,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            Event(() => ClerkProvidedTrackingNumber,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            Event(() => ClerkSentBox,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            Event(() => TruckDroveBox,
                configureEventCorrelation: x => x.CorrelateById(
                    c => c.Message.CorrelationId));

            // Initially defines story events that can create a state machine
            //  instance
            Initially(
                When(CustomerDroppedOffBox)
                    .Then(context =>
                    {
                        // You can access the Data from the message directly.
                        // You can also initialize the context's Instance data
                        //  to use if the saga will publish events with it.
                        context.Instance.CustomerName = context.Data.CustomerName;
                        context.Instance.AddressStreet = context.Data.AddressStreet;
                        context.Instance.AddressCity = context.Data.AddressCity;
                        context.Instance.AddressState = context.Data.AddressState;
                        context.Instance.AddressZip = context.Data.AddressZip;
                    })
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync(
                            $"Box received for {context.Instance.CustomerName} " +
                            $"going to {context.Instance.AddressCity}, {context.Instance.AddressState}");
                    })
                    // Kick off the first microservice
                    .Publish(context => new ClerkReceivedBoxEvent(context.Instance))
                    .TransitionTo(BoxInStore)
            );

            // Story where, while the box is in the store, the customer makes
            //  the payment and all is good.
            // Because events are used instead of commands, any microservice, as
            //  well as the saga, can subscribe and consume the event's
            //  messages.
            // So as you can see in this story branch, the orchestrator isn't
            //  doing much except reporting progress.
            During(BoxInStore,
                When(ClerkWeighedBox)
                    .Then(context =>
                        Console.Out.WriteLine(
                            $"Box x,y,z: " +
                            $"{{{context.Data.BoxDimensionsX}," +
                            $"{context.Data.BoxDimensionsY}," +
                            $"{context.Data.BoxDimensionsZ}}}")),
                When(ClerkProvidedTaxableInvoice)
                    .ThenAsync(async context =>
                        await Console.Out.WriteLineAsync(
                            $"Invoice total: ${context.Data.Total}")),
                When(CustomerPayedInvoice)
                    .ThenAsync(async context =>
                        await Console.Out.WriteLineAsync(
                            $"Customer paid invoice: {(context.Data.PaidInFull ? "Yes" : "No")}")),
                When(ClerkProvidedTrackingNumber)
                    .ThenAsync(async context =>
                        await Console.Out.WriteLineAsync(
                            $"Tracking number: {context.Data.TrackingNumber}")),
                When(ClerkSentBox)
                    .ThenAsync(async context =>
                        await Console.Out.WriteLineAsync(
                            $"Clerk sent box at: {context.Data.TimestampBoxLeftFacility}"))
                    .TransitionTo(BoxInTruck)
            );

            // Fault tolerance
            //  Story where, while the box is in the store, if the user does
            //      not pay, cancel the transaction and end the saga.
            //  You can branch off from the story without connecting event
            //      handling statements to the other branch chains above.
            //  Although there's a fluent style with the chain, there is no
            //      signifigance with the order of defining your story branches.
            //  Note the use of Finalize here. It ends the saga
            //      after this event is handled.
            During(BoxInStore,
                When(CustomerCancelledInvoice)
                    .ThenAsync(async context => await Console.Out.WriteLineAsync("Customer cancelled transaction."))
                    .Finalize()
            );

            // Note the use of Finalize here. It ends the saga after this event
            //  is handled.
            // Also note that you can do some publishing before finalizing the
            //  saga. Publishing events then would allow you to have
            //  rollback microservices dedicated to rolling back transactions to
            //  a previous state.
            During(BoxInTruck,
                When(TruckDroveBox)
                    .ThenAsync(async context =>
                        await Console.Out.WriteLineAsync(
                            $"Box delivered at: {context.Data.TimestampDeliveryCompleted}"))
                    .TransitionTo(BoxDelivered)
                    .Finalize()
            );

            // When the saga transaction is complete, remove it from the
            //  repository
            SetCompletedWhenFinalized();
        }
    }
}