namespace Stotele.Server.Models
{
    public class PrekesParduotuve
    {
        public int Id { get; set; }
        public int Kiekis { get; set; }
        public int ParduotuveId { get; set; }
        public Parduotuve Parduotuve { get; set; }
        public int PrekeId { get; set; }
        public Preke Preke { get; set; }
    }
}