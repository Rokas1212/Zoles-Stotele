namespace Stotele.Server.Models
{
    public class Parduotuve
    {
        public int Id { get; set; }
        public string DarboLaikoPradzia { get; set; }
        public string DarboLaikoPabaiga { get; set; }
        public double Kvadratura { get; set; }
        public int DarbuotojuKiekis { get; set; }
        public string TelNumeris { get; set; }
        public string ElPastas { get; set; }
        public string Faksas { get; set; }
        public string Adresas { get; set; }
    }
}