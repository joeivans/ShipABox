namespace ShipABox.Common.Contracts.Commands
{
    public interface ICustomerDroppedBoxCommand
    {
        string CustomerName { get; }
        string AddressStreet { get; }
        string AddressCity { get; }
        string AddressState { get; }
        string AddressZip { get; }
    }
}
