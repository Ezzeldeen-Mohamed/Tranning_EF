namespace MeterManagement.Application.Common
{

    /// <summary>
    /// Represents a standardized response for service operations.
    /// </summary>
    /// <typeparam name="T">The type of response data.</typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Indicates whether the operation succeeded.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Human-readable response message.
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Response payload.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// Validation or business errors.
        /// </summary>
        public IReadOnlyList<string> Errors { get; init; } = [];

        public static BaseResponse<T> Success(
            T data,
            string? message = null)
        {
            return new BaseResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static BaseResponse<T> Failure(
            string message,
            IEnumerable<string>? errors = null)
        {
            return new BaseResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors?.ToList() ?? []
            };
        }
    }
}
