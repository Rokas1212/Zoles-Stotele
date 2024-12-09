namespace Stotele.Server.Models
{
    public class Naudotojas
    {
        public int Id { get; set; }
        public string Lytis { get; set; }
        public string ElektroninisPastas { get; set; }
        public string Slaptazodis { get; set; }
        public string Vardas { get; set; }
        public string Slapyvardis { get; set; }
        public string Pavarde { get; set; }
        public bool Administratorius { get; set; }
    }
}