using AudioCoreApi.Models;
using AudioCoreSerial.I;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCoreApi.Services
{
    public class ResetService
    {
        private readonly IAmplifier amplifier;
        private readonly AudioContext dbContext;

        public ResetService(
            IAmplifier amplifier,
            AudioContext dbContext
        )
        {
            this.amplifier = amplifier;
            this.dbContext = dbContext;
        }

        public async Task ResetAsync()
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
        }
    }
}
