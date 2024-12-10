import React from "react";
import { addToCart } from "../../apiServices/cart";

const Prekes = () => {
  const products = [
    { id: "1", name: "Albaniška žolė", price: 10.0 },
    { id: "2", name: "Žolės stotelės firminė žolė", price: 15.0 },
    { id: "3", name: "Piktžolių veja", price: 20.0 },
  ];

  const handleAddToCart = async (product: any) => {
    try {
      await addToCart({
        productId: product.id,
        name: product.name,
        price: product.price,
        quantity: 1,
      });
      alert(`${product.name} pridėtą į krepšelį.`);
    } catch (error) {
      console.error("Klaida: ", error);
      alert("Nepavyko pridėti prekės į krepšelį.");
    }
  };

  return (
    <div>
      <h1>Prekės</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Pavadinimas</th>
            <th>Kaina</th>
            <th>Veiksmai</th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td>
                <a href={`/preke?id=${product.id}`}>{product.id}</a>
              </td>
              <td>{product.name}</td>
              <td>€{product.price.toFixed(2)}</td>
              <td>
                <button
                  onClick={() => handleAddToCart(product)}
                  className="btn btn-primary"
                >
                  Add to Cart
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Prekes;