using System.ComponentModel.DataAnnotations.Schema;

namespace AudioCoreApi.Models
{
    /// <summary>
    /// Class representing the inputs.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Input id.
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
        /// Indicates if the input should be hidden.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Checks if two inputs are the same. We look only at the Id.
        /// </summary>
        /// <param name="obj">Other object</param>
        /// <returns>True if they are equal</returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Input return false.
            Input i = obj as Input;
            if (i == null)
            {
                return false;
            }

            // Return true if the fields match:
            return i.Id == Id;
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
