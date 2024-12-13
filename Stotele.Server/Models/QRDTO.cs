namespace Stotele.Server.Models
{
    public class QRResponseDTO
    {
        public int OrderId { get; set; }
        public string QRCodeUrl { get; set; }
    }

    public class ProcessQRDTO
    {
        public int OrderId { get; set; }
    }
}
