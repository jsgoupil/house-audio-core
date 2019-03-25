using Newtonsoft.Json;

namespace AudioCoreApi.Models
{
    public class TemplateEntry
    {
        public int Id { get; set; }

        public int TemplateId { get; set; }
        [JsonIgnore]
        public virtual Template Template { get; set; }

        public int OutputId { get; set; }
        public int? LinkWithInputId { get; set; }
        public int? Volume { get; set; }
        public int? Bass { get; set; }
        public int? Treble { get; set; }
        public bool? OnState { get; set; }
    }
}
