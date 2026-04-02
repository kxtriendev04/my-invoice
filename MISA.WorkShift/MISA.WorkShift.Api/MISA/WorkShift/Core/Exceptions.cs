
namespace MISA.WorkShift.Core
{
    internal class Exceptions
    {
        [Serializable]
        internal class DuplicateException : Exception
        {
            public DuplicateException()
            {
            }

            public DuplicateException(string? message) : base(message)
            {
            }

            public DuplicateException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        [Serializable]
        internal class NotFoundException : Exception
        {
            public NotFoundException()
            {
            }

            public NotFoundException(string? message) : base(message)
            {
            }

            public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }
}