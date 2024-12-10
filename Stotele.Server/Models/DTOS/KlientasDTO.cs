namespace Stotele.Server.Models
{
    public class KlientasDTO : NaudotojasDTO
    {
        public int PastoKodas { get; set; }
        public string Miestas { get; set; }
        public DateTime GimimoData { get; set; }
        public string Adresas { get; set; }
    }
}