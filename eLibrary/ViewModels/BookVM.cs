using eLibrary.DataLayers.Entities;

namespace eLibrary.ViewModels
{
    public class BookVM
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Author { get; set; }
        public string BookPicURL { get; set; }
        public string Kategoria { get; set; }
    }
}
