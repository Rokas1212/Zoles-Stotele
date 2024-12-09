namespace Stotele.Server.Models
{
    public class MegstamaKategorija
    {
        public int Id { get; set; }
        public DateTime PridejimoData { get; set; }
        public int KategorijaId { get; set; }
        public Kategorija Kategorija { get; set; }
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
    }
}