namespace Stotele.Server.Models
{
    public class PrekesUzsakymas
    {
        public int Id { get; set; }
        public int Kiekis { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
        public int UzsakymasId { get; set; }
        public Uzsakymas Uzsakymas { get; set; }
    }
}