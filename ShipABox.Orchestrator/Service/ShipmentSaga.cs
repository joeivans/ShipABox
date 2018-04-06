using System;
using Automatonymous;
using ShipABox.Common.Contracts.Commands;
using ShipABox.Common.Contracts.Events;
using ShipABox.Orchestrator.Service.Events;

namespace ShipABox.Orchestrator.Service
{
    /**
     *  Seal the saga class because of virtual calls in the constructor
     */
    public sealed class ShipmentSaga : MassTransitStateMachine<ShipmentSagaState>
    {
        /**
         *  This class is where the orchestration happens.
         *
         *  Remember that this is a distributed system, and events can
         *  be observed out of order. This means that you should allow
         *  some overlap of your During statements, in case an event
         *  comes into the saga but
         *  the state hasn't switched yet.
         *
         *  Other things you should know:
         *      - There is only ever one instance of the Saga class.
         *
         *      - When many sagas are being orchestrated, the Saga class
         *        just switches to the current SagaState that it's
         *        working on. That SagaState can be accessed through the
         *        TInstance property.
         */


        /**
         *  Declare the possible sates this machine can be in.
         */
        public State BoxInStore { get; set; }
        public State BoxInTruck { get; set; }
        public State BoxDelivered { get; set; }


        /**
         *  Declare the events this saga subscribes to.
         *
         *  The saga doesn't need to react to every event, FYI.
         *
         *  However, if you wire up an event to a certain state but the
         *  event fires while the machine is in another state, you'll
         *  get an UnhandledEvent Exception.
         */
        public Event<ICustomerDroppedBoxCommand> CustomerDroppedOffBox { get; private set; }
        public Event<IClerkWeighedBoxEvent> ClerkWeighedBox { get; private set; }
        public Event<IClerkProvidedTaxableInvoiceEvent> ClerkProvidedTaxableInvoice { get; private set; }
        public Event<ICustomerPayedInvoiceEvent> CustomerPayedInvoice { get; private set; }
        public Event<IClerkProvidedTrackingNumberEvent> ClerkProvidedTrackingNumber { get; private set; }
        public Event<IClerkSentBoxEvent> ClerkSentBox { get; private set; }
        public Event<ICustomerCancelledInvoiceEvent> CustomerCancelledInvoice { get; private set; }
        public Event<ITruckDroveBoxEvent> TruckDroveBox { get; private set; }


        /**
         * The constructor is where all the magic happens.
         */
        public ShipmentSaga()
        {
            /**
             *  Set the state property, allowing the machine to track
             *  the state.
             */
            InstanceState(s => s.State);


            /**
             *  Wire up the events for this saga.
             *
             *  For the first correlation, use two fields that match in
             *  the saga state and the first message type. Then you have
             *  to tell the saga how to set its own Correlation Id. In
             *  this case we're giving it a new Guid.
             */
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


            /**
             *  We have to define our saga story by State + Event pairs.
             *
             *  The event's states must be defined, and you should allow
             *  some overlap to design the state machine to be a little
             *  forgiving. Remember that in a distributed system, the
             *  events flowing through the messaging bus are not
             *  guaranteed to be ordered. This means you'll have to add
             *  some logic to handle out-of-order messages. For example,
             *  the saga might observe the ClerkSentBox event before
             *  ClerkProvidedeTrackingNumber. You can use Composite
             *  Events to handle something like this, which are
             *  activated after every event in the composite definition
             *  have been observed.
             *
             *  Because events are used instead of commands, any
             *  microservice, as well as the saga, can subscribe and
             *  consume the event's messages.
             *
             */


            /**
             *  Initially defines story events that can create a state
             *  machine instance.
             */
            Initially(
                When(CustomerDroppedOffBox)
                    .Then(context =>
                    {
                        /**
                         *  context.Instance is the saga's state
                         *  properties i.e. the TInstance in this
                         *  orchestrator class.
                         *
                         *  context.Data is the event's message data
                         *  coming in to the saga.
                         *
                         *  If you don't assign the Data to the Instance
                         *  properties, the saga will not retain any of
                         *  that message's data.
                         */
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
                    /**
                     * Kick off the first microservice.
                     */
                    .Publish(context => new ClerkReceivedBoxEvent(context.Instance))
                    .TransitionTo(BoxInStore)
            );


            /**
             *  Story line where, while the box is in the store, the
             *  customer makes the payment and all is good.
             *
             *  Also notice that, as long as the events should occur
             *  during the same machine state, we declare the events as
             *  params to visually group the state/event combinations.
             */
            During(BoxInStore, BoxInTruck,
                When(ClerkWeighedBox)
                    .ThenAsync(
                        async context =>
                        {
                            await Console.Out.WriteLineAsync(
                                $"Box x,y,z: " +
                                $"{{{context.Data.BoxDimensionsX}," +
                                $"{context.Data.BoxDimensionsY}," +
                                $"{context.Data.BoxDimensionsZ}}}");
                        }),
                When(ClerkProvidedTaxableInvoice)
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync(
                            $"Invoice total: ${context.Data.Total}");
                    }),
                When(CustomerPayedInvoice)
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync(
                            $"Customer paid invoice: {(context.Data.PaidInFull ? "Yes" : "No")}");
                    }),
                When(ClerkProvidedTrackingNumber)
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync(
                            $"Tracking number: {context.Data.TrackingNumber}");
                    }),
                When(ClerkSentBox)
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync(
                            $"Clerk sent box at: {context.Data.TimestampBoxLeftFacility}");
                    })
                    .TransitionTo(BoxInTruck)
            );


            /**
             *  Fault tolerance
             *
             *  Story where, while the box is in the store, if the user
             *  does not pay, cancel the transaction and end the saga.
             *
             *  You can branch off from the story without connecting
             *  event handling statements to the other branch chains
             *  above.
             *
             *  Although there's a fluent style with the chain, there is
             *  no signifigance with the order of defining your story
             *  branches.
             *
             *  Note the use of Finalize here. It ends the saga after
             *  this event is handled.
             */
            During(BoxInStore,
                When(CustomerCancelledInvoice)
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync("Customer cancelled transaction.");
                    })
                    .Finalize()
            );


            /**
             *  Note the use of Finalize here. It ends the saga after
             *  this event is handled.
             *
             *  Also note that you can do some publishing before
             *  finalizing the saga. Publishing events then would allow
             *  you to have rollback microservices dedicated to rolling
             *  back transactions to a previous state.
             *
             *  Transitioning to the BoxDelivered state doesn't really
             *  do anything in this case because it's the final step
             *  anyway. But we could've added an email notification
             *  microservice as the next step if we wanted to.
             */
            During(BoxInStore, BoxInTruck,
                When(TruckDroveBox)
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync(
                            $"Box delivered at: {context.Data.TimestampDeliveryCompleted}");
                    })
                    .TransitionTo(BoxDelivered)
                    .Finalize()
            );


            /**
             * When the saga transaction is complete, remove it from the
             *  repository
             */
            SetCompletedWhenFinalized();
        }
    }
}