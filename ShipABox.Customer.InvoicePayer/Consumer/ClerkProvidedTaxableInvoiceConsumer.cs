using System;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using ShipABox.Common.Contracts.Events;

namespace ShipABox.Customer.InvoicePayer.Consumer
{
    public class ClerkProvidedTaxableInvoiceConsumer :
        IConsumer<IClerkProvidedTaxableInvoiceEvent>
    {
        /**
         *  ClerkProvidedTaxableInvoiceConsumer
         *
         *  -   Defines how to consume the event's message.
         *
         *  -   Publishes another event when this microservice is done,
         *      signaling the start of next unit of work for the next
         *      service.
         */


        /**
         *  Message template for console.
         */
        private const string StrOut = "Paid the amount: ${0}";


        /**
         *  Required interface implementation.
         */
        public async Task Consume(ConsumeContext<IClerkProvidedTaxableInvoiceEvent> context)
        {
            /**
             *  Setup.
             */
            var fake = new Faker();
            var timestampPaymentInitiated = DateTimeOffset.Now;
            var customerActuallyPaidYn = fake.Random.Bool();
            var paidInFull = customerActuallyPaidYn;


            /**
             *  In a realistic scenario, there must be fault tolerance.
             *
             *  So here, there is a realistic scenario where the
             *  customer decides not to pay.
             *
             *  If the customer pays as expected in a transaction,
             *  everything continues as normal.
             *
             *  If the customer does not pay, then we need to recover
             *  from this by displaying an error message and finalizing
             *  the saga of this transaction.
             *
             *  You can think of this as fault tolerance. We're not
             *  throwing any exceptions here or handling exceptions but
             *  you can see from this simple example how you can just
             *  catch an exception and handle it in the event consumer
             *  class.
             */


            switch (customerActuallyPaidYn)
            {
                case true:


                    /**
                     *  Console out to user.
                     */
                    Console.WriteLine(StrOut, context.Message.Total);


                    /**
                     *  Publish event.
                     */
                    await context.Publish<ICustomerPayedInvoiceEvent>(
                        new
                        {
                            CorrelationId = context.Message.CorrelationId,
                            CustomerName = context.Message.CustomerName,
                            AddressStreet = context.Message.AddressStreet,
                            AddressCity = context.Message.AddressCity,
                            AddressState = context.Message.AddressState,
                            AddressZip = context.Message.AddressZip,
                            BoxDimensionsX = context.Message.BoxDimensionsX,
                            BoxDimensionsY = context.Message.BoxDimensionsY,
                            BoxDimensionsZ = context.Message.BoxDimensionsZ,
                            BoxWeight = context.Message.BoxWeight,
                            BoxWeightUnit = context.Message.BoxWeightUnit,
                            SubTotal = context.Message.SubTotal,
                            Tax = context.Message.Tax,
                            Total = context.Message.Total,
                            TimestampInvoiceCreated =
                                context.Message.TimestampInvoiceCreated,

                            PaidInFull = paidInFull,
                            TimestampPaymentInitiated = timestampPaymentInitiated
                        });


                    break;
                default:


                    /**
                     *  Console out to user.
                     */
                    Console.WriteLine("Customer cancelled the transaction.");


                    /**
                     *  Publish event.
                     */
                    await context.Publish<ICustomerCancelledInvoiceEvent>(
                        new
                        {
                            CorrelationId = context.Message.CorrelationId,
                            CustomerName = context.Message.CustomerName,
                            AddressStreet = context.Message.AddressStreet,
                            AddressCity = context.Message.AddressCity,
                            AddressState = context.Message.AddressState,
                            AddressZip = context.Message.AddressZip,
                            BoxDimensionsX = context.Message.BoxDimensionsX,
                            BoxDimensionsY = context.Message.BoxDimensionsY,
                            BoxDimensionsZ = context.Message.BoxDimensionsZ,
                            BoxWeight = context.Message.BoxWeight,
                            BoxWeightUnit = context.Message.BoxWeightUnit,
                            SubTotal = context.Message.SubTotal,
                            Tax = context.Message.Tax,
                            Total = context.Message.Total,
                            TimestampInvoiceCreated =
                                context.Message.TimestampInvoiceCreated,

                            PaidInFull = paidInFull,
                            TimestampPaymentInitiated = timestampPaymentInitiated
                        });


                    break;
            }


            Console.WriteLine("Press enter to exit");
            Console.Write(">");
        }
    }
}