namespace Stotele.Server.Models
{
    public class Preke
    {
        public int Id { get; set; }
        public double Kaina { get; set; }
        public string Pavadinimas { get; set; }
        public int Kodas { get; set; }
        public DateTime GaliojimoData { get; set; }
        public int Kiekis { get; set; }
        public string Ismatavimai { get; set; }
        public string NuotraukosUrl { get; set; }
        public DateTime GarantinisLaikotarpis { get; set; }
        public string Aprasymas { get; set; }
        public double RekomendacijosSvoris { get; set; }
        public double Mase { get; set; }
        public int VadybininkasId { get; set; }
        public Vadybininkas Vadybininkas { get; set; }
    }
}