namespace ArasTrader.Application.Common
{
    public static class ApplicationErrorCodes
    {
        public const string CustomerNotFound = "Customer not found.";
        public const string WalletNotFound = "Wallet not found.";
        public const string CannotEditOrder = "Cannot edit order.";
        public const string OrderNotFound = "Order not found.";
        public const string ConcurrencyException = "Concurrency exception happened.";
        public const string CannotRetrieveToken = "Cannot retrieve token.";
        public const string CannotRetrieveCustomers = "Cannot retrieve customers.";

    }
}
