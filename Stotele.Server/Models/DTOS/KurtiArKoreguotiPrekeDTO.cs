public class KurtiArKoreguotiPrekeDTO
{
    public double Kaina { get; set; }
    public string Pavadinimas { get; set; }
    public DateTime GaliojimoData { get; set; }
    public string Aprasymas { get; set; }
    public string Ismatavimai { get; set; }
    public string NuotraukosUrl { get; set; }
    public int Kodas { get; set; }
    public int Kiekis { get; set; }
    public DateTime GarantinisLaikotarpis { get; set; }
    public double RekomendacijosSvoris { get; set; }
    public double Mase { get; set; }
    public int VadybininkasId { get; set; }
    public List<PrekesParduotuveDTO> PrekiuParduotuves { get; set; } = new List<PrekesParduotuveDTO>();
}

public class PrekesParduotuveDTO
{
    public int ParduotuveId { get; set; }
    public int Kiekis { get; set; }
}