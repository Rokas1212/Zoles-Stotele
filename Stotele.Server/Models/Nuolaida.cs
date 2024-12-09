namespace Stotele.Server.Models
{
    public class Nuolaida
    {
        public int Id { get; set; }
        public int Procentai { get; set; }
        public DateTime PabaigosData { get; set; }
        public int UzsakymasId { get; set; }
        public Uzsakymas Uzsakymas { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
    }
}