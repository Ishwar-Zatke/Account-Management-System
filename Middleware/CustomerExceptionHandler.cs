namespace Account_Management.Middleware
{
    public class CustomerExceptionHandler: Exception
    {
        public int StatusCode { get; }
        public string StatusMessage { get; }
        public int? borrowerIndex { get; }

        public CustomerExceptionHandler(string message, int statusCode) : base(message)
        {

            StatusMessage = message;
            StatusCode = statusCode;
        }

        public CustomerExceptionHandler(string message, int statusCode, int? index) : base(message)
        {
            StatusMessage = message;
            StatusCode = statusCode;
            borrowerIndex = index;
        }
    }
}
