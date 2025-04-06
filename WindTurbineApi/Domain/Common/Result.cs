namespace WindTurbineApi.Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }
        public bool IsNotFound { get; }
        public IEnumerable<string> Errors { get; } = Enumerable.Empty<string>();

        protected Result(bool isSuccess, T? value, string? error, bool isNotFound = false, IEnumerable<string>? errors = null)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("A successful result cannot have an error.");
            if (!isSuccess && value != null)
                throw new InvalidOperationException("A failed result cannot have a value.");
            if (isNotFound && isSuccess)
                throw new InvalidOperationException("A result cannot be both successful and not found.");
            if (isNotFound && error != null)
                throw new InvalidOperationException("A 'not found' result should not have a general error.");

            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            IsNotFound = isNotFound;
            if (errors != null)
            {
                Errors = errors;
            }
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error);
        }

        public static Result<T> NotFound()
        {
            return new Result<T>(false, default, null, true);
        }
    }

    // Non-generic Result for operations that don't return a value like Delete
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string? Error { get; protected set; }
        public bool IsNotFound { get; protected set; }

        protected Result(bool isSuccess, string? error = null, bool isNotFound = false)
        {
            IsSuccess = isSuccess;
            Error = error;
            IsNotFound = isNotFound;
        }

        public static Result Success() => new Result(true);
        public static Result Failure(string error) => new Result(false, error);
        public static Result NotFound() => new Result(false, null, true);
    }
}