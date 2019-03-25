using System.ComponentModel.DataAnnotations.Schema;

namespace AudioCoreApi.Models
{

    /// <summary>
    /// Class representing the zones.
    /// </summary>
    public class Output
    {
        /// <summary>
        /// Zone id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Friendly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicates if the zone is on.
        /// </summary>
        public bool On { get; set; }

        /// <summary>
        /// Volume from 0 to 100.
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Bass level from 0 to 100.
        /// </summary>
        public int Bass { get; set; }

        /// <summary>
        /// Treble from 0 to 100.
        /// </summary>
        public int Treble { get; set; }

        /// <summary>
        /// Indicates if the zone is muted.
        /// </summary>
        public bool Mute { get; set; }

        /// <summary>
        /// Linked Input
        /// </summary>
        public int? LinkInput { get; set; }

        /// <summary>
        /// Indicates if the output should be hidden.
        /// </summary>
        public bool Hidden { get; set; }
    }
}
