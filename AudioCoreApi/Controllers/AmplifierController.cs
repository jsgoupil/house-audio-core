using AudioCoreApi.Models;
using AudioCoreApi.Services;
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
        private readonly ICommunication communication;
        private readonly ResetService resetService;
        private readonly AudioContext dbContext;

        public AmplifierController(
            IAmplifier amplifier,
            ICommunication communication,
            ResetService resetService,
            AudioContext dbContext
        )
        {
            this.amplifier = amplifier;
            this.communication = communication;
            this.resetService = resetService;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets the version of the amplifier.
        /// </summary>
        /// <returns>Version</returns>
        [HttpGet]
        [Route("settings")]
        public IActionResult Settings()
        {
            return Ok(new
            {
                communication.PortName 
            });
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
            await resetService.ResetAsync();
            return NoContent();
        }
    }
}
