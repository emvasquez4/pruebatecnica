using System;
namespace TarjetasCredito.Modelos
{
    public class UpdateSaldosException : Exception
    {
        public UpdateSaldosException()
        {
        }

        public UpdateSaldosException(string message)
        : base(message)
        {
        }

        public UpdateSaldosException(string message, Exception innerException)
            : base(message + " " + innerException.Message)
        {
        }
    }
}
