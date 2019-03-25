using System.Threading.Tasks;

namespace AudioCoreSerial.I
{
    /// <summary>
    /// Communication interface for reading/writing.
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// Write data.
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Asynchrounous task.</returns>
        Task WriteAsync(string data);

        /// <summary>
        /// Read data.
        /// </summary>
        /// <returns>Data read asynchronously.</returns>
        Task<string> ReadAsync();
    }
}
