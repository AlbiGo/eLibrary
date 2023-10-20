using System.ComponentModel.DataAnnotations;

namespace eLibrary.DataLayers.Entities
{
    public class BaseModel
    {
        [Key]
        public int ID { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
