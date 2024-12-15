namespace Stotele.Server.Models
{
    public class RedaguotiNaudotojasDTO
    {
        public string? Vardas { get; set; }
        public string? ElektroninisPastas { get; set; }
        public string? Slaptazodis { get; set; }
        public string? Lytis { get; set; }
        public RedaguotiKlientasDTO? Klientas { get; set; }
    }

    public class RedaguotiKlientasDTO
    {
        public string? Miestas { get; set; }
        public string? Adresas { get; set; }
        public int? PastoKodas { get; set; }
        public DateTime? GimimoData { get; set; }
    }
}
