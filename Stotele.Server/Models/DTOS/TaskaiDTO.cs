namespace Stotele.Server.Models
{
    public class TaskaiDTO
    {
        public int Id { get; set; }
        public DateTime PabaigosData { get; set; }
        public int Kiekis { get; set; }
        public int KlientasId { get; set; }
        public string KlientasVardas { get; set; }
        public string KlientasPavarde { get; set; }
        public int? ApmokejimasId { get; set; }
        public decimal? GalutineSuma { get; set; }
    }
}