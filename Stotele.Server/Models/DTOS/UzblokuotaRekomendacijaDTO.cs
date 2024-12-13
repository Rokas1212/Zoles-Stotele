namespace Stotele.Server.Models
{
    public class UzblokuotosRekomendacijos
    {
        public int Id { get; set; }
        public int KlientasId { get; set; }
        public int PrekeId { get; set; }
        public string NuotraukosUrl { get; set; }
        public string Pavadinimas { get; set; }
        public string Aprasymas { get; set; }
    }
}