namespace Stotele.Server.Models
{
    public class UzblokuotaRekomendacija
    {
        public int Id { get; set; }
        public DateTime PridejimoData { get; set; }
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
    }
}