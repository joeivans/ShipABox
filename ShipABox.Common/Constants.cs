namespace ShipABox.Common
{
    public static class Constants
    {
        /**
         *  This component creates a common kernel library for the
         *  entire solution to use.
         */


        /**
         *  Bus configuration.
         */
        public const string Uri = "rabbitmq://localhost/shipabox/";
        public const string Username = "guest";
        public const string Password = "guest";


        /**
         *  Queue names.
         */
        public const string OrchestratorQueue = "shipabox.orchestrator";
        public const string BoxWeigherQueue = "shipabox.clerk.boxweigher";
        public const string InvoiceProviderQueue = "shipabox.clerk.invoiceprovider";
        public const string InvoicePayerQueue = "shipabox.customer.invoicepayer";
        public const string TrackingNumberProviderQueue = "shipabox.clerk.trackingnumberprovider";
        public const string BoxSenderQueue = "shipabox.clerk.boxsender";
        public const string BoxDriverQueue = "shipabox.truck.boxdriver";
    }
}