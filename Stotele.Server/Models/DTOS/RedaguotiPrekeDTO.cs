public class RedaguotiPrekeDTO
{
    public string Pavadinimas { get; set; }
    public double Kaina { get; set; }
    public DateTime GaliojimoData { get; set; }
    public string Aprasymas { get; set; }

    public string NuotraukosUrl { get; set; }
    public DateTime GarantinisLaikotarpis { get; set; }

    public List<PrekesKategorijaDTO> PrekiuKategorijos { get; set; } = new List<PrekesKategorijaDTO>();
}
