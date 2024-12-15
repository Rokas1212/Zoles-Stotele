// Krepselio valdiklis - cart controller
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stotele.Server.Models.SessionModels;
using Stotele.Server.Models.ApplicationDbContexts;


public class AddToCartPayload
{
    public string id { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class KrepselioController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public KrepselioController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    private const string CartSessionKey = "ShoppingCart";

    [HttpGet]
    public ActionResult<List<CartItem>> GetCart()
    {
        var cart = GetCartFromSession();
        return Ok(cart);
    }

    [HttpPost("add")]
    public ActionResult AddToCart([FromBody] AddToCartPayload payload)
    {
        string productId = payload.id;

        // Fetch product details from the database
        var product = _dbContext.Prekes.FirstOrDefault(p => p.Id.ToString() == productId);
        if (product == null)
        {
            return NotFound($"Produktas su tokiu ID: {productId} nerastas.");
        }
        if (product.Kiekis == 0)
        {
            return BadRequest("Produktas yra išparduotas.");
        }
        // Retrieve the current cart from session
        var cart = GetCartFromSession();

        // Check if the product is already in the cart
        var existingItem = cart.FirstOrDefault(i => i.id == productId);
        if (existingItem != null)
        {
            existingItem.kiekis++;
        }
        else
        {
            cart.Add(new CartItem
            {
                id = product.Id.ToString(),
                pavadinimas = product.Pavadinimas,
                kaina = product.Kaina,
                kiekis = 1
            });
        }

        // Save the updated cart to session
        SaveCartToSession(cart);

        return Ok(cart); // Return updated cart
    }

    [HttpPost("remove")]
    public ActionResult RemoveFromCart([FromBody] string id)
    {
        var cart = GetCartFromSession();
        cart.RemoveAll(i => i.id == id);
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