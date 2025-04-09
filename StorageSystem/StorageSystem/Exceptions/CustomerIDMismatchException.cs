namespace StorageSystem.Exceptions
{
    [Serializable]
    internal class CustomerIDMismatchException : Exception
    {
        public CustomerIDMismatchException()
        {
        }

        public CustomerIDMismatchException(string? message) : base(message)
        {
        }

        public CustomerIDMismatchException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}