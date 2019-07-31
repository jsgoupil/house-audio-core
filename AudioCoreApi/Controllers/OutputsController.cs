using AudioCoreApi.Exceptions;
using AudioCoreApi.Models;
using AudioCoreSerial.I;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutputsController : ControllerBase
    {
        private readonly IAmplifier amplifier;
        private readonly AudioContext dbContext;

        public OutputsController(
            IAmplifier amplifier,
            AudioContext dbContext
        )
        {
            this.amplifier = amplifier;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets the total amount of outputs we support.
        /// ID starts at 0.
        /// </summary>
        /// <returns></returns>
        [HttpGet("amount")]
        public int GetAmount()
        {
            return amplifier.GetOutputAmount();
        }

        /// <summary>
        /// Lists all outputs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IEnumerable<Output>> List()
        {
            var list = new List<Output>();
            var amount = amplifier.GetOutputAmount();
            for (var i = 0; i < amount; i++)
            {
                list.Add(await GetOutputAsync(i, createIfNotExist: true));
            }

            return list;
        }

        /// <summary>
        /// Gets an output based on its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Output> Get(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output;
        }

        /// <summary>
        /// Sets the name of an output.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/name")]
        public async Task<IActionResult> PostName(int id, WebApiInput<string> input)
        {
            CheckId(id);

            if (input == null)
            {
                return BadRequest();
            }

            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Name = input.Input;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Gets the name of an output.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/name")]
        public async Task<string> GetName(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.Name;
        }

        /// <summary>
        /// Sets the order of an output.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/order")]
        public async Task<IActionResult> PostOrder(int id, WebApiInput<int> input)
        {
            CheckId(id);

            if (input == null)
            {
                return BadRequest();
            }

            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Order = input.Input;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Gets the order of an output.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/order")]
        public async Task<int> GetOrder(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.Order ?? 0;
        }

        /// <summary>
        /// Hides the input.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/hide")]
        public async Task<IActionResult> Hide(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Hidden = true;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Unhides the input.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/unhide")]
        public async Task<IActionResult> Unhide(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Hidden = false;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Gets the state, true if it ON.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/on")]
        public async Task<bool> GetOn(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.On ?? false;
        }

        /// <summary>
        /// Sets the state to ON.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/on")]
        public async Task<IActionResult> PostOn(int id)
        {
            CheckId(id);

            await amplifier.SetOnStateAsync(id, true);
            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.On = true;
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Sets the state to OFF.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/off")]
        public async Task<IActionResult> PostOff(int id)
        {
            CheckId(id);

            await amplifier.SetOnStateAsync(id, false);
            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.On = false;
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Gets the treble.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/treble")]
        public async Task<int> GetTreble(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.Treble ?? 0;
        }

        /// <summary>
        /// Sets the treble.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/treble")]
        public async Task<IActionResult> PostTreble(int id, WebApiInput<int> input)
        {
            CheckId(id);

            if (input == null)
            {
                return BadRequest();
            }

            await amplifier.SetTrebleAsync(id, input.Input);
            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Treble = input.Input;
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Gets the bass.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/bass")]
        public async Task<int> GetBass(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.Bass ?? 0;
        }

        /// <summary>
        /// Sets the bass.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/bass")]
        public async Task<IActionResult> PostBass(int id, WebApiInput<int> input)
        {
            CheckId(id);

            if (input == null)
            {
                return BadRequest();
            }

            await amplifier.SetBassAsync(id, input.Input);
            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Bass = input.Input;
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/volume")]
        public async Task<int> GetVolume(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.Volume ?? 0;
        }

        /// <summary>
        /// Sets the volume.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/volume")]
        public async Task<IActionResult> PostVolume(int id, WebApiInput<int> input)
        {
            CheckId(id);

            if (input == null)
            {
                return BadRequest();
            }

            await amplifier.SetVolumeAsync(id, input.Input);
            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.Volume = input.Input;
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Bring the volume up.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("volumeup")]
        public async Task<IActionResult> VolumeUp(WebApiInput<int> input)
        {
            var volumeChange = input?.Input ?? 20;
            foreach (var output in await dbContext.Outputs.ToListAsync())
            {
                output.Volume = Math.Min(100, output.Volume + volumeChange);
                await amplifier.SetVolumeAsync(output.Id, output.Volume);
            }

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Bring the volume up. For LUUP.
        /// </summary>
        /// <returns></returns>
        [HttpGet("volumeup")] // LUUP has trouble to do POST
        public Task<IActionResult> VolumeUp()
        {
            return VolumeUp(new WebApiInput<int>
            {
                Input = 10
            });
        }

        /// <summary>
        /// Bring the volume down.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("volumedown")]
        public async Task<IActionResult> VolumeDown(WebApiInput<int> input)
        {
            var volumeChange = input?.Input ?? 20;
            foreach (var output in await dbContext.Outputs.ToListAsync())
            {
                output.Volume = Math.Max(0, output.Volume - volumeChange);
                await amplifier.SetVolumeAsync(output.Id, output.Volume);
            }

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Bring the volume down. For LUUP.
        /// </summary>
        /// <returns></returns>
        [HttpGet("volumedown")] // LUUP has trouble to do POST
        public Task<IActionResult> VolumeDown()
        {
            return VolumeDown(new WebApiInput<int>
            {
                Input = 10
            });
        }

        /// <summary>
        /// Reads the link.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/link")]
        public async Task<int?> GetLink(int id)
        {
            CheckId(id);

            var output = await GetOutputAsync(id, createIfNotExist: true);
            return output?.LinkInput;
        }

        /// <summary>
        /// Sets the link.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputId"></param>
        /// <returns></returns>
        [HttpPost("{id}/link/{inputId}")]
        public async Task PostLink(int id, int inputId)
        {
            CheckId(id);

            await amplifier.LinkAsync(inputId, id);
            var output = await GetOutputAsync(id, createIfNotExist: true);
            output.LinkInput = inputId;
            await dbContext.SaveChangesAsync();
        }

        private async Task<Output> GetOutputAsync(int id, bool createIfNotExist)
        {
            var output = await dbContext.Outputs.FirstOrDefaultAsync(m => m.Id == id);
            if (output == null && createIfNotExist)
            {
                output = new Output
                {
                    Id = id,
                    Order = 0,
                    Name = $"Output {id}",
                };
                dbContext.Outputs.Add(output);
                await dbContext.SaveChangesAsync();
            }

            return output;
        }

        private void CheckId(int id)
        {
            if (id < 0 || id > amplifier.GetOutputAmount() - 1)
            {
                throw new InexistentResourceException();
            }
        }
    }
}