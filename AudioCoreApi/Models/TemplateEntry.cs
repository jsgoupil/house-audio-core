using System.Collections.Generic;

namespace AudioCoreApi.Models
{
    public class Template
    {
        public Template()
        {
            TemplateEntries = new HashSet<TemplateEntry>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<TemplateEntry> TemplateEntries { get; set; }
    }
}
