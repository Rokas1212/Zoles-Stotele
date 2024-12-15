using System.ComponentModel.DataAnnotations;
using Stotele.Server.Models;

public class PrekeDTO
{
    public int Id { get; set; }
    public string Pavadinimas { get; set; }
    public double Kaina { get; set; }
    public string Aprasymas { get; set; }
    public string NuotraukosUrl { get; set; }
    public int Kodas { get; set; }
    public List<PrekesKategorija> PrekesKategorijos { get; set; }
    public string Ismatavimai { get; set; }
    public Double Mase { get; set; }

    public DateTime GarantinisLaikotarpis { get; set; }
    public DateTime GaliojimoData { get; set; }
}
