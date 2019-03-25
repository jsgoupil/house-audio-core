using System.Threading.Tasks;

namespace AudioCoreSerial.I
{

    /// <summary>
    /// Interface for any kind of amplifier.
    /// </summary>
    public interface IAmplifier
    {
        int GetOutputAmount();
        int GetInputAmount();
        Task<bool> GetOnStateAsync(int outputId);
        Task<bool> GetMuteAsync(int outputId);
        Task<int> GetTrebleAsync(int outputId);
        Task<int> GetBassAsync(int outputId);
        Task<int> GetVolumeAsync(int outputId);
        Task SetOnStateAsync(int outputId, bool on);
        Task SetMuteStateAsync(int outputId, bool on);
        Task SetTrebleAsync(int outputId, int value);
        Task SetBassAsync(int outputId, int value);
        Task SetVolumeAsync(int outputId, int value);
        Task LinkAsync(int inputId, int outputId);
        Task<string> GetVersionAsync();
        Task ResetAsync();
    }
}
