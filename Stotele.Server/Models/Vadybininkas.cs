namespace Stotele.Server.Models
{
    public class Vadybininkas
    {
        public int Id { get; set; }
        public string Skyrius { get; set; }
        public int NaudotojasId { get; set; }
        public Naudotojas Naudotojas { get; set; }
        public int ParduotuveId { get; set; }
        public Parduotuve Parduotuve { get; set; }

        public ICollection<Kategorija> Kategorijos { get; set; }
    }
}