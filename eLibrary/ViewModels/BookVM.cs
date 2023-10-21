using eLibrary.DataLayers.Entities;

namespace eLibrary.ViewModels
{
    public class BookVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Author { get; set; }
    }
}
