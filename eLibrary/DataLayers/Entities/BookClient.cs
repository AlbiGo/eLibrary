using System.ComponentModel.DataAnnotations.Schema;

namespace eLibrary.DataLayers.Entities
{
    public class BookClient : BaseModel
    {
        public DateTime BorrowedDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        #region Client Relationship

        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public Client Client { get; set; }

        #endregion

        #region Book RelationShip

        [ForeignKey("Book")]
        public int BookID { get; set; }
        public Book Book { get; set; }

        #endregion
    }
}
