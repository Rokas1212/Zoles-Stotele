using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Stotele.Server.Models
{
    public class PrekesUzsakymas
    {
        public int Id { get; set; }
        public int Kiekis { get; set; }

        [ForeignKey("PrekeId")]
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }

        [ForeignKey("UzsakymasId")]
        public int UzsakymasId { get; set; }

        [JsonIgnore]
        public Uzsakymas Uzsakymas { get; set; }
    }
}