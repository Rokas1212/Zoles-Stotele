namespace Stotele.Server.Models
{
    public class Kategorija
    {
        public int Id { get; set; }
        public string Pavadinimas { get; set; }
        public string Aprasymas { get; set; }
        public int VadybininkasId { get; set; }
        public Vadybininkas Vadybininkas { get; set; }
    }
}