namespace ArasTrader.Domain.Common
{
    public static class DomainErrorCodes
    {
        public const string CustomerNotFound = "Customer not found.";
        public const string InsufficientBalance = "Insufficient balance.";
        public const string InvalidAmount = "Invalid amount.";
        public const string InvalidSymbol = "Invalid symbol.";
        public const string InvalidQuantity = "Invalid quantity.";
        public const string InvalidPrice = "Invalid price.";
        public const string CannotCreateCustomer = "Can not create customer.";
        public const string InvalidOrderState = "Invalid order state.";
    }
}
