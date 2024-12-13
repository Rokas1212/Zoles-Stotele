using System.ComponentModel.DataAnnotations.Schema;

namespace Stotele.Server.Models
{
    public class Apmokejimas
    {
        public int Id { get; set; }
        public int SaskaitosFakturosNumeris { get; set; }
        public string PvmMoketojoKodas { get; set; }
        public double GalutineSuma { get; set; }
        public int PanaudotiTaskai { get; set; }
        public int PridetiTaskai { get; set; }
        public ApmokejimoMetodas ApmokejimoMetodas { get; set; }
        public MokejimoStatusas MokejimoStatusas { get; set; }
        [ForeignKey("Klientas")]
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
        [ForeignKey("Uzsakymas")]
        public int UzsakymasId { get; set; }
        public Uzsakymas Uzsakymas { get; set; }
    }
}