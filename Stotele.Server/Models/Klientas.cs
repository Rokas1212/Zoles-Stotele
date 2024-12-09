namespace Stotele.Server.Models
{
    public class Klientas
    {
        public int Id { get; set; }
        public int PastoKodas { get; set; }
        public string Miestas { get; set; }
        public DateTime GimimoData { get; set; }
        public string Adresas { get; set; }
        public int NaudotojasId { get; set; }
        public Naudotojas Naudotojas { get; set; }
    }
}