using System.ComponentModel.DataAnnotations.Schema;

namespace eLibrary.DataLayers.Entities
{
    public class Book : BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }

        #region Author Relationship

        [ForeignKey("Author")]
        public int AuthorID { get; set; }
        public Author Author { get; set; }

        #endregion
    }
}
