using System;




namespace WebApplication1.Models
{
    public class InternalServerException : Exception
    {
        public InternalServerException()
        {
        }

        public InternalServerException(string message)
            : base(message)
        {
        }

        public InternalServerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public int StatusCode => 500;
    }
}
