namespace Stotele.Server.Models
{
    public class Adresas
    {
        public int Id { get; set; }
        public string Gatve { get; set; }
        public int NamoNumeris { get; set; }
        public int ButoNumeris { get; set; }
        public string PastoKodas { get; set; }
        public string Miestas { get; set; }
        public string Salis { get; set; }
        public int NaudotojasId { get; set; }
        public Naudotojas Naudotojas { get; set; }
        public int KlientasId { get; set; }
        public Klientas Klientas { get; set; }
    }
}