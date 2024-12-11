import React, { useEffect, useState } from "react";
import { addToCart } from "../../apiServices/cart";
import Loading from "../../components/loading";

const Prekes = () => {
  const [products, setProducts] = useState<any[]>([]); // State to hold the list of Prekes
  const [loading, setLoading] = useState<boolean>(true); // Loading state
  const [error, setError] = useState<string | null>(null); // Error state

  // Fetch the list of Prekes from the backend
  useEffect(() => {
    const fetchPrekes = async () => {
      try {
        const response = await fetch("https://localhost:5210/api/Preke/PrekesList", {
          credentials: "include",
        });

        if (!response.ok) {
          throw new Error("Nepavyko gauti prekių.");
        }
        const data = await response.json();
        setProducts(data);
      } catch (error) {
        console.error("Klaida:", error);
        setError("Nepavyko gauti prekių.");
      } finally {
        setLoading(false);
      }
    };

    fetchPrekes();
  }, []);

  const handleAddToCart = async (id: string) => {
    try {
      await addToCart(id);
      alert(`Prekė su ID ${id} pridėta į krepšelį.`);
    } catch (error) {
      console.error("Klaida: ", error);
      alert("Nepavyko pridėti prekės į krepšelį.");
    }
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div>{error}</div>;
  }

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
              <td>{product.pavadinimas}</td>
              <td>€{product.kaina.toFixed(2)}</td>
              <td>
                <button
                  onClick={() => handleAddToCart(product.id)}
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