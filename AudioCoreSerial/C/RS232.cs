using AudioCoreSerial.I;
using RJCP.IO.Ports;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AudioCoreSerial.C
{
    /// <summary>
    /// RS232 implementation of the ICommunication
    /// </summary>
    public class RS232 : ICommunication, IDisposable
    {
        private static readonly object obj = new Object();

        /// <summary>
        /// Instance of the serial port.
        /// </summary>
        private readonly SerialPortStream serialPort;

        /// <summary>
        /// Delay in ms.
        /// </summary>
        private readonly int writeDelay;

        public string PortName { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="portName">RS232 Port</param>
        public RS232(
            string portName,
            int writeDelay
        )
        {
            PortName = portName;
            serialPort = new SerialPortStream(portName)
            {
                BaudRate = 19200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ReadTimeout = 1000,
                WriteTimeout = 500,
                Handshake = Handshake.None
            };
            this.writeDelay = writeDelay;
        }

        /// <summary>
        /// Connects on the port.
        /// </summary>
        public void Connect()
        {
            lock (obj)
            {
                if (!serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Open();
                    }
                    catch (IOException ex)
                    {
                        throw new SerialPortException("Can't open the connection on serial port", ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new SerialPortException("Can't open the connection on serial port", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Disconnects.
        /// </summary>
        public void Disconnect()
        {
            lock (obj)
            {
                serialPort.Close();
            }
        }

        /// <summary>
        /// Write on the port. It connects if needed.
        /// </summary>
        /// <param name="data">Data to write</param>
        /// <returns>Asynchronous task.</returns>
        public Task WriteAsync(string data)
        {
            Connect();

            lock (obj)
            {
                serialPort.Write(data);
            }

            return Task.Delay(writeDelay);
        }

        /// <summary>
        /// Reads on the port. It connects if needed.
        /// </summary>
        /// <returns>Data read asynchronously.</returns>
        public Task<string> ReadAsync()
        {
            Connect();
            lock (obj)
            {
                return Task.FromResult(serialPort.ReadLine());
            }
        }

        /// <summary>
        /// Read data, until the RS232 times out.
        /// </summary>
        /// <returns>Data read asynchronously.</returns>
        public Task<string> ReadUntilTimeoutAsync()
        {
            var str = new StringBuilder();

            Connect();
            lock (obj)
            {
                do
                {
                    try
                    {
                        str.AppendLine(serialPort.ReadLine());
                    }
                    catch (TimeoutException)
                    {
                        break;
                    }
                } while (true);
            }

            return Task.FromResult(str.ToString());
        }

        /// <summary>
        /// Disposes.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}