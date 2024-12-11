namespace Stotele.Server.Models
{
    public class NuolaidaDTO
    {
        public int Id { get; set; }
        public int Procentai { get; set; }
        public DateTime GaliojimoPabaiga { get; set; }
        public string PrekesPavadinimas { get; set; }
        public double PrekesKaina { get; set; }
        public double PrekesKainaPoNuolaidos => PrekesKaina * (100 - Procentai) / 100;
    }
}