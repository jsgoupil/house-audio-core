using AudioCoreApi.Exceptions;
using AudioCoreApi.Models;
using AudioCoreSerial.I;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InputsController : ControllerBase
    {
        private readonly IAmplifier amplifier;
        private readonly AudioContext dbContext;

        public InputsController(
            IAmplifier amplifier,
            AudioContext dbContext
        )
        {
            this.amplifier = amplifier;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets the total amount of input.
        /// </summary>
        /// <returns></returns>
        [HttpGet("amount")]
        public int GetAmount()
        {
            return amplifier.GetInputAmount();
        }

        /// <summary>
        /// Lists all inputs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IEnumerable<Input>> List()
        {
            var list = new List<Input>();
            var amount = amplifier.GetInputAmount();
            for (var i = 0; i < amount; i++)
            {
                list.Add(await GetInputAsync(i, createIfNotExist: true));
            }

            return list;
        }

        /// <summary>
        /// Gets the input.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            CheckId(id);

            var input = await GetInputAsync(id, createIfNotExist: true);
            return Ok(input);
        }

        /// <summary>
        /// Sets the input name.
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

            var inp = await GetInputAsync(id, createIfNotExist: true);
            inp.Name = input.Input;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Gets the input name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/name")]
        public async Task<string> GetName(int id)
        {
            CheckId(id);

            var input = await GetInputAsync(id, createIfNotExist: true);
            return input?.Name;
        }

        /// <summary>
        /// Sets the input order.
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

            var inp = await GetInputAsync(id, createIfNotExist: true);
            inp.Order = input.Input;
            await dbContext.SaveChangesAsync();
            return NoContent();
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

            var input = await GetInputAsync(id, createIfNotExist: true);
            input.Hidden = true;
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

            var input = await GetInputAsync(id, createIfNotExist: true);
            input.Hidden = false;
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Gets the input order.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/order")]
        public async Task<int> GetOrder(int id)
        {
            CheckId(id);

            var input = await GetInputAsync(id, createIfNotExist: true);
            return input?.Order ?? 0;
        }

        private async Task<Input> GetInputAsync(int id, bool createIfNotExist)
        {
            var input = await dbContext.Inputs.FirstOrDefaultAsync(m => m.Id == id);
            if (input == null && createIfNotExist)
            {
                input = new Input
                {
                    Id = id,
                    Order = 0,
                    Name = $"Input {id}",
                };
                dbContext.Inputs.Add(input);
                await dbContext.SaveChangesAsync();
            }

            return input;
        }

        private void CheckId(int id)
        {
            if (id < 0 || id > amplifier.GetInputAmount() - 1)
            {
                throw new InexistentResourceException();
            }
        }
    }
}