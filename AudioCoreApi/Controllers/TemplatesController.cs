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
    public class TemplatesController : ControllerBase
    {
        private readonly IAmplifier amplifier;
        private readonly AudioContext dbContext;

        public TemplatesController(
            IAmplifier amplifier,
            AudioContext dbContext
        )
        {
            this.amplifier = amplifier;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets all the templates.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Template>> Get()
        {
            var templates = await dbContext.Templates
                .Include(m => m.TemplateEntries)
                .ToListAsync();

            return templates;
        }

        /// <summary>
        /// Gets a template.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var template = await dbContext.Templates
                .Include(m => m.TemplateEntries)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        /// <summary>
        /// Deletes a template.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await dbContext.Templates.FirstOrDefaultAsync(m => m.Id == id);
            if (template != null)
            {
                dbContext.Templates.Remove(template);
                await dbContext.SaveChangesAsync();
            }

            return NoContent();
        }
        
        /// <summary>
        /// Creates a template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Template> Create([FromBody]Template template)
        {
            dbContext.Templates.Add(template);
            await dbContext.SaveChangesAsync();
            return template;
        }

        /// <summary>
        /// Deletes a template entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateEntryId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/entries/{templateEntryId}")]
        public async Task<IActionResult> DeleteTemplateEntry(int id, int templateEntryId)
        {
            var templateEntry = await dbContext.TemplateEntries.FirstOrDefaultAsync(m => m.TemplateId == id && m.Id == templateEntryId);
            if (templateEntry != null)
            {
                dbContext.TemplateEntries.Remove(templateEntry);
                await dbContext.SaveChangesAsync();
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a template entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateEntry"></param>
        /// <returns></returns>
        [HttpPost("{id}/entries")]
        public async Task<IActionResult> CreateTemplateEntry(int id, [FromBody]TemplateEntry templateEntry)
        {
            var template = await dbContext.Templates
                .Include(m => m.TemplateEntries)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (template == null)
            {
                return NotFound();
            }

            template.TemplateEntries.Add(templateEntry);
            await dbContext.SaveChangesAsync();

            return Ok(templateEntry);
        }

        /// <summary>
        /// Apply a template.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/apply")] // LUUP has trouble to do POST
        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyTemplate(int id)
        {
            var template = await dbContext.Templates
                .Include(m => m.TemplateEntries)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (template == null)
            {
                return NotFound();
            }

            // We only apply if the amplifier has a different value.
            foreach (var entry in template.TemplateEntries)
            {
                var output = await dbContext.Outputs.FirstOrDefaultAsync(m => m.Id == entry.OutputId);
                if (output == null)
                {
                    continue;
                }

                if (entry.Bass.HasValue)
                {
                    if (output.Bass != entry.Bass.Value)
                    {
                        await amplifier.SetBassAsync(entry.OutputId, entry.Bass.Value);
                        output.Bass = entry.Bass.Value;
                    }
                }

                if (entry.Treble.HasValue)
                {
                    if (output.Treble != entry.Treble.Value)
                    {
                        await amplifier.SetTrebleAsync(entry.OutputId, entry.Treble.Value);
                        output.Treble = entry.Treble.Value;
                    }
                }

                if (entry.Volume.HasValue)
                {
                    if (output.Volume != entry.Volume.Value)
                    {
                        await amplifier.SetVolumeAsync(entry.OutputId, entry.Volume.Value);
                        output.Volume = entry.Volume.Value;
                    }
                }

                if (entry.OnState.HasValue)
                {
                    if (output.On != entry.OnState.Value)
                    {
                        await amplifier.SetOnStateAsync(entry.OutputId, entry.OnState.Value);
                        output.On = entry.OnState.Value;
                    }
                }

                if (entry.LinkWithInputId.HasValue)
                {
                    if (!output.LinkInput.HasValue || output.LinkInput != entry.LinkWithInputId.Value)
                    {
                        await amplifier.LinkAsync(entry.LinkWithInputId.Value, entry.OutputId);
                        output.LinkInput = entry.LinkWithInputId.Value;
                    }
                }
            }

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}