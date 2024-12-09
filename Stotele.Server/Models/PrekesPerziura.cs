namespace Stotele.Server.Models
{
    public class PrekesPerziura
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int Kiekis { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
    }
}