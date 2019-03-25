using AudioCoreApi.Models;
using AudioCoreSerial.I;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AudioControllerCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmplifierController : ControllerBase
    {
        private readonly IAmplifier amplifier;
        private readonly AudioContext dbContext;

        public AmplifierController(
            IAmplifier amplifier,
            AudioContext dbContext
        )
        {
            this.amplifier = amplifier;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets the version of the amplifier.
        /// </summary>
        /// <returns>Version</returns>
        [HttpGet]
        [Route("version")]
        public async Task<string> Version()
        {
            return await amplifier.GetVersionAsync();
        }

        /// <summary>
        /// Resets the amplifier.
        /// </summary>
        /// <returns>Async.</returns>
        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> Reset()
        {
            var outputs = await dbContext.Outputs.ToListAsync();
            foreach (var output in outputs)
            {
                await amplifier.SetBassAsync(output.Id, output.Bass);
                await amplifier.SetTrebleAsync(output.Id, output.Treble);
                await amplifier.SetVolumeAsync(output.Id, output.Volume);
                await amplifier.SetOnStateAsync(output.Id, output.On);

                if (output.LinkInput.HasValue)
                {
                    await amplifier.LinkAsync(output.LinkInput.Value, output.Id);
                }
            }

            return NoContent();
        }
    }
}
