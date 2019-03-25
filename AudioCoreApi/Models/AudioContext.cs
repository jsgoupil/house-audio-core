using Microsoft.EntityFrameworkCore;

namespace AudioCoreApi.Models
{
    /// <summary>
    /// DbContext for AE6MC.
    /// </summary>
    public class AudioContext : DbContext
    {
        public DbSet<Output> Outputs { get; set; }
        public DbSet<Input> Inputs { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateEntry> TemplateEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=audio.db");
        }
    }
}
