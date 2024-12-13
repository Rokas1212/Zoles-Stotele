namespace Stotele.Server.Models
{
    public class MegstamaKategorijaDTO
    {
        public int Id { get; set; }
        public DateTime PridejimoData { get; set; }
        public int KategorijaId { get; set; }
        public string KategorijaPavadinimas { get; set; }
        public string KategorijaAprasymas { get; set; }
    }
}