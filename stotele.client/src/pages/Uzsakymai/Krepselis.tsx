import { useEffect, useState } from "react";
import { fetchCart, createOrder, CartItem, removeFromCart } from "../../apiServices/cart";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./krepselis.css";

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
        toast.error("Nepavyko gauti krepšelio duomenų.");
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
      toast.success("Užsakymas sėkmingai sukurtas!");
      navigate(`/uzsakymas/${response.orderId}`);
    } catch (error) {
      console.error("Klaida:", error);
      toast.error("Nepavyko sukurti užsakymo.");
    }
  };

  const handleRemoveFromCart = async (id: string) => {
    try {
      const updatedCart = await removeFromCart(id);
      setCart(updatedCart);
      toast.success("Prekė pašalinta iš krepšelio.");
    } catch (error) {
      console.error("Klaida:", error);
      toast.error("Nepavyko pašalinti prekės iš krepšelio.");
    }
  };

  return (
    <div className="krepselis-container">
      <h1 className="title">Krepšelis</h1>
      {cart.length > 0 ? (
        <div>
          <div className="table-container">
            <table className="styled-table">
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
                        className="remove-button"
                      >
                        Pašalinti
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="summary-section">
            <h3>Bendra suma: €{getTotalPrice()}</h3>
            <button onClick={handleCreateOrder} className="create-order-button">
              Sukurti Užsakymą
            </button>
          </div>
        </div>
      ) : (
        <p className="empty-cart-message">Krepšelis tuščias.</p>
      )}
    </div>
  );
};

export default Krepselis;
