namespace Stotele.Server.Models
{
    public class Uzsakymas
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public double Suma { get; set; }

        public ICollection<PrekesUzsakymas> PrekesUzsakymai { get; set; } = new List<PrekesUzsakymas>();
    }
}