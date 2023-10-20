namespace eLibrary.DataLayers.Entities
{
    public class Client : BaseModel
    {
        public string FirstName { get; set; }
        public string LastnName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }    
        public DateTime DataRegjistrimit { get; set; }
        public bool IsAdmin { get; set; }
    }
}
