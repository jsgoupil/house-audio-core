using System;

namespace AudioCoreSerial
{
    public class SerialPortException : Exception
    {
        public SerialPortException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
