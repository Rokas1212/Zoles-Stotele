namespace Stotele.Server.Models
{
    public class PrekesKategorija
    {
        public int Id { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
        public int KategorijaId { get; set; }
        public Kategorija Kategorija { get; set; }
    }
}