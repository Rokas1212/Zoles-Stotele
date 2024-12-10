namespace Stotele.Server.Models.SessionModels
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}