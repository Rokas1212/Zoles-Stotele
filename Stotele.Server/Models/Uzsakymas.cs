using System.ComponentModel.DataAnnotations.Schema;

namespace Stotele.Server.Models
{
    public class Uzsakymas
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public double Suma { get; set; }

        public bool Patvirtintas { get; set; } = false;

        [ForeignKey("Naudotojas")]
        public int NaudotojasId { get; set; }
        public Naudotojas Naudotojas { get; set; }

        public ICollection<PrekesUzsakymas> PrekesUzsakymai { get; set; } = new List<PrekesUzsakymas>();
    }
}