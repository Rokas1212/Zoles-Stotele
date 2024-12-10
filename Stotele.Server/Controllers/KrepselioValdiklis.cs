// Krepselio valdiklis - cart controller
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stotele.Server.Models.SessionModels;

[ApiController]
[Route("api/[controller]")]
public class KrepselioController : ControllerBase
{
    private const string CartSessionKey = "ShoppingCart";

    [HttpGet]
    public ActionResult<List<CartItem>> GetCart()
    {
        var cart = GetCartFromSession();
        return Ok(cart);
    }

    [HttpPost("add")]
    public ActionResult AddToCart([FromBody] CartItem item)
    {
        var cart = GetCartFromSession();
        var existingItem = cart.Find(i => i.ProductId == item.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Add(item);
        }

        cart.ForEach(i => Console.WriteLine($"Product: {i.Name}, Quantity: {i.Quantity}, Price: {i.Price}, ProductId: {i.ProductId}"));
        SaveCartToSession(cart);
        return Ok(cart);
    }

    [HttpPost("remove")]
    public ActionResult RemoveFromCart([FromBody] string productId)
    {
        var cart = GetCartFromSession();
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCartToSession(cart);
        return Ok(cart);
    }

    [HttpPost("clear")]
    public ActionResult ClearCart()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return Ok();
    }

    private List<CartItem> GetCartFromSession()
    {
        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
    }

    private void SaveCartToSession(List<CartItem> cart)
    {
        var cartJson = JsonConvert.SerializeObject(cart);
        HttpContext.Session.SetString(CartSessionKey, cartJson);
    }
}