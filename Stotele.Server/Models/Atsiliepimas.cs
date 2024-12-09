namespace Stotele.Server.Models
{
    public class Atsiliepimas
    {
        public int Id { get; set; }
        public string Komentaras { get; set; }
        public int IvercioVerte { get; set; }
        public DateTime Data { get; set; }
        public string Titulas { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
    }
}