using AudioCoreSerial.I;
using System;
using System.Threading.Tasks;

namespace AudioCoreSerial.C
{
    /// <summary>
    /// Controls the AE6MC.
    /// </summary>
    public class ControlAE6MC : IAmplifier
    {
        /// <summary>
        /// Number of available zones.
        /// </summary>
        private const int NumberOfOutputs = 6;

        /// <summary>
        /// Number of available inputs.
        /// </summary>
        private const int NumberOfInputs = 7;

        /// <summary>
        /// Communication protocol
        /// </summary>
        private ICommunication communication;

        /// <summary>
        /// Control Constructor
        /// </summary>
        /// <param name="communication">Communication class</param>
        public ControlAE6MC(ICommunication communication)
        {
            this.communication = communication;
        }

        /// <summary>
        /// Gets the version of the amplifier.
        /// </summary>
        /// <returns>Amplifier version</returns>
        public async Task<string> GetVersionAsync()
        {
            await communication.WriteAsync("(vr?)");
            return await communication.ReadAsync();
        }

        /// <summary>
        /// Resets the amplifier.
        /// </summary>
        /// <returns>Async</returns>
        public async Task ResetAsync()
        {
            await communication.WriteAsync("(rx)");
        }

        /// <summary>
        /// Turns on or off a zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="on">On or off</param>
        /// <returns>Async</returns>
        public async Task SetOnStateAsync(int outputId, bool on)
        {
            var index = GetIndexFromId(outputId);
            await (on ?
                communication.WriteAsync("(" + index + "on)") :
                communication.WriteAsync("(" + index + "of)"));
        }

        /// <summary>
        /// Mutes or unmutes a zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="on">Muted (true) or Unmuted (false)</param>
        /// <returns>Async</returns>
        public async Task SetMuteStateAsync(int outputId, bool on)
        {
            var index = GetIndexFromId(outputId);
            await (on ?
                communication.WriteAsync("(" + index + "mu)") :
                communication.WriteAsync("(" + index + "um)"));
        }

        /// <summary>
        /// Mutes all the zones.
        /// </summary>
        /// <param name="mute">Muted (true) or Unmuted (false)</param>
        /// <returns>Async</returns>
        public async Task MuteAll(bool mute)
        {
            await (mute ?
                communication.WriteAsync("(amu)") :
                communication.WriteAsync("(aum)"));
        }

        /// <summary>
        /// Gets the volume for a specific zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <returns>Volume from 0 to 100</returns>
        public async Task<int> GetVolumeAsync(int outputId)
        {
            var index = GetIndexFromId(outputId);
            await communication.WriteAsync("(" + index + "vl?)");
            string result = await communication.ReadAsync();
            int finalResult;
            if (!int.TryParse(result, out finalResult))
            {
                return -1;
            }

            return finalResult * 100 / 87;
        }

        /// <summary>
        /// Sets the volume for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="value">Volume from 0 to 100</param>
        /// <returns>Async</returns>
        public async Task SetVolumeAsync(int outputId, int value)
        {
            // Our value goes from 0 to 87
            var index = GetIndexFromId(outputId);
            await communication.WriteAsync("(" + index + "vl" + (value * 87 / 100).ToString("D2") + ")");
        }

        /// <summary>
        /// Gets the bass for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <returns>Bass from 0 to 100</returns>
        public async Task<int> GetBassAsync(int outputId)
        {
            var index = GetIndexFromId(outputId);
            await communication.WriteAsync("(" + index + "b?)");
            return Convert.ToInt32(await communication.ReadAsync(), 16);
        }

        /// <summary>
        /// Sets the bass for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="value">Bass from 0 to 100</param>
        /// <returns>Async</returns>
        public async Task SetBassAsync(int outputId, int value)
        {
            var index = GetIndexFromId(outputId);
            await communication.WriteAsync("(" + index + "b" + value.ToString("X") + ")");
        }

        /// <summary>
        /// Gets the treble for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <returns>Treble from 0 to 100</returns>
        public async Task<int> GetTrebleAsync(int outputId)
        {
            var index = GetIndexFromId(outputId);
            await communication.WriteAsync("(" + index + "t?)");
            return Convert.ToInt32(await communication.ReadAsync(), 16);
        }

        /// <summary>
        /// Sets the treble for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="value">Treble from 0 to 100</param>
        /// <returns>Async</returns>
        public async Task SetTrebleAsync(int outputId, int value)
        {
            var index = GetIndexFromId(outputId);
            await communication.WriteAsync("(" + index + "t" + value.ToString("X") + ")");
        }

        /// <summary>
        /// Links zone to an input.
        /// </summary>
        /// <param name="id">Zone</param>
        /// <param name="input">Input</param>
        /// <returns>Async</returns>
        public async Task LinkAsync(int inputId, int outputId)
        {
            var indexOutput = GetIndexFromId(outputId);
            var indexInput = GetIndexFromId(inputId);
            await communication.WriteAsync("(" + indexOutput + "sl" + indexInput + ")");
        }

        public int GetOutputAmount()
        {
            return NumberOfOutputs;
        }

        public int GetInputAmount()
        {
            return NumberOfInputs;
        }

        public Task<bool> GetMuteAsync(int outputId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetOnStateAsync(int outputId)
        {
            throw new NotImplementedException();
        }

        private int GetIndexFromId(int id)
        {
            return id + 1;
        }
    }
}
