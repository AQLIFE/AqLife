namespace MyLife.Core.Func
{
    public class APIException : Exception
    {
        public enum APIErrorCode
        {
            InternalServerError = 0,
            InvalidRequest = 1,
            Unauthorized = 2,
            ResourceNotFound = 3
        }

        public enum AppErrorCode
        {
            ConfigError = 0,
            NetwordError = 1,
            TypeError = 2,
            ActionError = 3
        }
        public APIException(string message) : base(message)
        {
        }

        public APIException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}