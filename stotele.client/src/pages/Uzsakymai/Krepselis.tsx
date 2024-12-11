import React, { useEffect, useState } from "react";
import { fetchCart, CartItem } from "../../apiServices/cart";

const Krepselis = () => {
  const [cart, setCart] = useState<CartItem[]>([]);

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
              </tr>
            </thead>
            <tbody>
              {cart.map((item) => (
                <tr key={item.id}>
                  <td>{item.pavadinimas}</td>
                  <td>€{item.kaina.toFixed(2)}</td>
                  <td>{item.kiekis}</td>
                  <td>€{(item.kaina * item.kiekis).toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <h3>Bendra suma: €{getTotalPrice()}</h3>
          <a href="/uzsakymas" className="btn btn-primary">
            Sukurti Užsakymą
          </a>
        </div>
      ) : (
        <p>Krepšelis tuščias.</p>
      )}
    </div>
  );
};

export default Krepselis;