namespace ArasTrader.Domain.Common
{
    public class Result<T>
    {
        public T Value { get; private set; }
        public bool IsSuccess { get; private set; }
        public IReadOnlyCollection<DomainError> Errors { get; set; } = new List<DomainError>();

        protected Result(T value, bool isSuccess, IReadOnlyCollection<DomainError> errors)
        {
            Value = value;
            IsSuccess = isSuccess;
            Errors = errors;
        }

        private Result()
        {

        }

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                Value = value,
                IsSuccess = true
            };
        }

        public static Result<T> Failure(DomainError error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Errors = new List<DomainError>() { error }
            };
        }

        public static Result<T> Failure(List<DomainError> errors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Errors = errors
            };
        }
    }

    public record DomainError(string Code, string Message);
}
