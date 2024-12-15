import React, { useEffect, useState } from "react";
import { addToCart } from "../../apiServices/cart";
import Loading from "../../components/loading";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./prekes.css";
import { useNavigate } from "react-router-dom";

interface Product {
  id: string;
  pavadinimas: string;
  kaina: number;
}

const Prekes: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

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
      toast.success(
        <div className="d-flex align-items-center justify-content-between">
          <span className="mb-0">Prekė pridėta</span>
          <button
            className="btn btn-primary btn-sm"
            onClick={() => {
              navigate('/krepselis');
              toast.dismiss();
            }}
          >
            Eiti į krepšelį
          </button>
        </div>,
        {
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: false,
          pauseOnHover: true,
          draggable: true,
        }
      );
    } catch (error) {
      console.error("Klaida: ", error);
      toast.error("Nepavyko pridėti prekės į krepšelį.", {
        position: "top-right",
        autoClose: 3000,
      });
    }
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="error-message">{error}</div>;
  }

  return (
    <div className="prekes-container">
      <h1 className="title">Prekės</h1>
      <div className="table-container">
        <table className="styled-table">
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
                <td>{product.id}</td>
                <td>{product.pavadinimas}</td>
                <td>€{product.kaina.toFixed(2)}</td>
                <td>
                  <button
                    onClick={() => handleAddToCart(product.id)}
                    className="action-button add-to-cart-btn"
                  >
                    Pridėti į krepšelį
                  </button>
                  <a
                    href={`/preke?id=${product.id}`}
                    className="action-button view-product-btn"
                  >
                    Peržiūrėti prekę
                  </a>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Prekes;
