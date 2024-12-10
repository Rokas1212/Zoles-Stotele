namespace Stotele.Server.Models
{
    public class CreateNaudotojasDTO
    {
        public string Lytis { get; set; }
        public string ElektroninisPastas { get; set; }
        public string Slaptazodis { get; set; }
        public string Vardas { get; set; }
        public string Slapyvardis { get; set; }
        public string Pavarde { get; set; }
        public bool Administratorius { get; set; }
        public bool IsKlientas { get; set; }
        public int KlientasPastoKodas { get; set; }
        public string KlientasMiestas { get; set; }
        public DateTime KlientasGimimoData { get; set; }
        public string KlientasAdresas { get; set; }
        public bool IsVadybininkas { get; set; }
        public string VadybininkasSkyrius { get; set; }
        public int VadybininkasParduotuveId { get; set; }
    }
}