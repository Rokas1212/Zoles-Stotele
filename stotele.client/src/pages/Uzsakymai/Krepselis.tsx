import { useEffect, useState } from "react";
import { fetchCart, createOrder, CartItem, removeFromCart } from "../../apiServices/cart";
import { useNavigate } from "react-router-dom";

const Krepselis = () => {
  const [cart, setCart] = useState<CartItem[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadCart = async () => {
      try {
        const cartData = await fetchCart();
        setCart(cartData);
      } catch (error) {
        console.error("Klaida:", error);
      }
    };

    loadCart();
  }, []);

  const getTotalPrice = () => {
    return cart.reduce((total, item) => total + item.kaina * item.kiekis, 0).toFixed(2);
  };

    
  const handleCreateOrder = async () => {
    try {
      const response = await createOrder(cart);
      navigate(`/uzsakymas/${response.orderId}`);
    } catch (error) {
      console.error("Klaida:", error);
      alert("Nepavyko sukurti užsakymo.");
    }
  };
  

  const handleRemoveFromCart = async (id: string) => {
    try {
      const updatedCart = await removeFromCart(id);
      setCart(updatedCart);
    } catch (error) {
      console.error("Klaida:", error);
      alert("Nepavyko pašalinti prekės iš krepšelio.");
    }
  };

  return (
    <div>
      <h1>Krepšas</h1>
      {cart.length > 0 ? (
        <div>
          <table className="table">
            <thead>
              <tr>
                <th>Prekė</th>
                <th>Kaina</th>
                <th>Kiekis</th>
                <th>Iš viso</th>
                <th>Veiksmai</th>
              </tr>
            </thead>
            <tbody>
              {cart.map((item) => (
                <tr key={item.id}>
                  <td>{item.pavadinimas}</td>
                  <td>€{item.kaina.toFixed(2)}</td>
                  <td>{item.kiekis}</td>
                  <td>€{(item.kaina * item.kiekis).toFixed(2)}</td>
                  <td>
                    <button
                      onClick={() => handleRemoveFromCart(item.id)}
                      className="btn btn-danger"
                    >
                      Pašalinti
                    </button>
                    </td>
                </tr>
              ))}
            </tbody>
          </table>
          <h3>Bendra suma: €{getTotalPrice()}</h3>
          <button onClick={handleCreateOrder} className="btn btn-primary">
            Sukurti Užsakymą
          </button>
        </div>
      ) : (
        <p>Krepšelis tuščias.</p>
      )}
    </div>
  );
};

export default Krepselis;