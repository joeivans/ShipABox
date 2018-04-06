namespace ShipABox.Common.Contracts.Commands
{
    public interface ICustomerDroppedBoxCommand
    {
        /**
         *  A common interface for events where the Customer visits the
         *  store and drops off a box to the Clerk.
         */


        /**
         *  All the properties required for this event to be useful.
         */
        string CustomerName { get; }
        string AddressStreet { get; }
        string AddressCity { get; }
        string AddressState { get; }
        string AddressZip { get; }
    }
}