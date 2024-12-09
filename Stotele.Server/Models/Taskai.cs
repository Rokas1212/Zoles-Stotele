namespace Stotele.Server.Models
{
    public class Taskai
    {
        public int Id { get; set; }
        public DateTime PabaigosData { get; set; }
        public int Kiekis { get; set; }
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
        public int ApmokejimasId { get; set; }
        public Apmokejimas Apmokejimas { get; set; }
    }
}