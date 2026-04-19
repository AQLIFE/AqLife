namespace MyLife.Core.Func
{
    public class APIException : Exception
    {
        public enum APIErrorCode
        {
            UnknownError = 0,
            InvalidRequest = 1,
            Unauthorized = 2,
            ResourceNotFound = 3,
            InternalServerError = 4
        }
        public APIException(string message) : base(message)
        {
        }

        public APIException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}