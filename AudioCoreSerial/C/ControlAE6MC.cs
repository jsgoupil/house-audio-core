using AudioCoreSerial.I;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace AudioCoreSerial.C
{
    /// <summary>
    /// Controls the AE6MC.
    /// This controller will always reply "Error 1: No ~ detected" after a successful command.
    /// In case the command is unsucessful, it will reply "Error 2: Unrecognized Command".
    /// If you enter the volume higher than 88, it will RESET the amplifier.
    /// </summary>
    public class ControlAE6MC : IAmplifier
    {
        private const string NORMAL_LINE = "Error 1: No ~ detected";
        private const int MAX_VOLUME = 87;

        /// <summary>
        /// The hex like is used because the amplifier return something similar to HEX but shows the value in ASCII.
        /// For instance, 3= signifies 60 in decimal.
        /// </summary>
        private static readonly char[] HEX_LIKE = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?' };

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
        private readonly ICommunication communication;

        private readonly ILogger logger;

        /// <summary>
        /// Control Constructor
        /// </summary>
        /// <param name="communication">Communication class</param>
        public ControlAE6MC(
            ICommunication communication,
            ILogger<ControlAE6MC> logger
        )
        {
            this.communication = communication;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the version of the amplifier.
        /// </summary>
        /// <returns>Amplifier version</returns>
        public async Task<string> GetVersionAsync()
        {
            logger.LogTrace("Sending {MESSAGE}", "(vr?)");
            await communication.WriteAsync("(vr?)");
            return await communication.ReadUntilTimeoutAsync();
        }

        /// <summary>
        /// Resets the amplifier.
        /// </summary>
        /// <returns>Async</returns>
        public async Task ResetAsync()
        {
            logger.LogTrace("Sending {MESSAGE}", "(rx)");
            await communication.WriteAsync("(rx)");
            await WaitForReply();
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
            var message = on ? "(" + index + "on)" : "(" + index + "of)";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
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
            var message = on ? "(" + index + "mu)" : "(" + index + "um)";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
        }

        /// <summary>
        /// Mutes all the zones.
        /// </summary>
        /// <param name="mute">Muted (true) or Unmuted (false)</param>
        /// <returns>Async</returns>
        public async Task MuteAll(bool mute)
        {
            var message = mute ? "(amu)" : "(aum)";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
        }

        /// <summary>
        /// Gets the volume for a specific zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <returns>Volume from 0 to 100</returns>
        public async Task<int> GetVolumeAsync(int outputId)
        {
            var index = GetIndexFromId(outputId);
            var message = "(" + index + "vl?)";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await Task.Delay(100);
            var result = await communication.ReadAsync();
            logger.LogTrace("Receiving {MESSAGE}", result);

            // The value return is HEX like.
            // But it is immediately followed by Error 1: No ~ detected
            // Lowest number is 00 to send in.

            if (result.EndsWith(NORMAL_LINE))
            {
                var volume = result.Substring(0, 2);
                var volumePosition0 = Array.IndexOf(HEX_LIKE, volume[0]);
                var volumePosition1 = Array.IndexOf(HEX_LIKE, volume[1]);
                if (volumePosition0 >= 0 && volumePosition1 >= 1)
                {
                    var decValue = volumePosition0 * 16 + volumePosition1;

                    return decValue * 100 / MAX_VOLUME;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sets the volume for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="value">Volume from 0 to 100</param>
        /// <returns>Async</returns>
        public async Task SetVolumeAsync(int outputId, int value)
        {
            // Our value goes from 0 to MAX_VOLUME
            var index = GetIndexFromId(outputId);
            var message = "(" + index + "vl" + (value * MAX_VOLUME / 100).ToString("D2") + ")";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
        }

        /// <summary>
        /// Gets the bass for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <returns>Bass from 0 to 15</returns>
        public Task<int> GetBassAsync(int outputId)
        {
            throw new NotImplementedException("This amplifier cannot query the bass.");
        }

        /// <summary>
        /// Sets the bass for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="value">Bass from 0 to 15</param>
        /// <returns>Async</returns>
        public async Task SetBassAsync(int outputId, int value)
        {
            if (value < 0 || value > 15)
            {
                throw new ArgumentException(nameof(value), "The value must be between 0 and 15");
            }

            var index = GetIndexFromId(outputId);
            var message = "(" + index + "b" + value.ToString("x") + ")";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
        }

        /// <summary>
        /// Gets the treble for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <returns>Treble from 0 to 15</returns>
        public Task<int> GetTrebleAsync(int outputId)
        {
            throw new NotImplementedException("This amplifier cannot query the treble.");
        }

        /// <summary>
        /// Sets the treble for the zone.
        /// </summary>
        /// <param name="outputId">Zone</param>
        /// <param name="value">Treble from 0 to 100</param>
        /// <returns>Async</returns>
        public async Task SetTrebleAsync(int outputId, int value)
        {
            if (value < 0 || value > 15)
            {
                throw new ArgumentException(nameof(value), "The value must be between 0 and 15");
            }

            var index = GetIndexFromId(outputId);
            var message = "(" + index + "t" + value.ToString("x") + ")";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
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
            var message = "(" + indexOutput + "sl" + indexInput + ")";
            logger.LogTrace("Sending {MESSAGE}", message);
            await communication.WriteAsync(message);
            await WaitForReply();
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

        private Task WaitForReply()
        {
            /* We do nothing! */
            return Task.CompletedTask;
            /*
            try
            {
                var result = await communication.ReadAsync(); // Ignore the next line.
                logger.LogTrace("Received {MESSAGE}", result);
            }
            catch (TimeoutException)
            {
                logger.LogWarning("We timed out readining.");
            }*/
        }
    }
}