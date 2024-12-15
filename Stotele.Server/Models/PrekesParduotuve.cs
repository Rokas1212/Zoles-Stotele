using System.ComponentModel.DataAnnotations.Schema;

namespace Stotele.Server.Models
{
    public class PrekesParduotuve
    {
        public int Id { get; set; }
        public int Kiekis { get; set; }
        [ForeignKey("Parduotuve")]
        public int ParduotuveId { get; set; }
        public Parduotuve Parduotuve { get; set; }
        [ForeignKey("Preke")]
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
    }
}