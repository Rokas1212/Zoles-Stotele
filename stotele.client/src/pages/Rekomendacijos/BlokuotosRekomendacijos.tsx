import React, { useState, useEffect } from "react";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./blokuotosRekomendacijos.css";

type Product = {
  id: number;
  klientasId: number;
  prekeId: number;
  nuotraukosUrl: string;
  pavadinimas: string;
  aprasymas: string;
};

const BlokuotosRekomendacijos: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [unblocking, setUnblocking] = useState<number | null>(null);

  useEffect(() => {
    const user = localStorage.getItem("user");
    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
    } else {
      setError("Naudotojas neprisijungęs");
      setLoading(false);
    }
  }, []);

  const fetchData = async () => {
    if (!userId) return;

    setLoading(true);
    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/blokuotos-rekomendacijos/${userId}`
      );
      if (!response.ok) throw new Error("Nėra blokuotų rekomendacijų.");

      const data: Product[] = await response.json();
      setProducts(data.sort((a, b) => a.id - b.id));
    } catch (err: any) {
      console.error("Klaida:", err);
      setError(err.message || "Nepavyko gauti duomenų.");
    } finally {
      setLoading(false);
    }
  };

  const handleUnblockRecommendation = async (productId: number) => {
    setUnblocking(productId);
    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/blokuotos-rekomendacijos/atblokuoti/${userId}/${productId}`,
        { method: "DELETE" }
      );

      if (!response.ok) throw new Error("Nepavyko atblokuoti rekomendacijos.");

      toast.success("Rekomendacija sėkmingai atblokuota!", {
        position: "top-right",
        autoClose: 2000,
      });

      fetchData();
    } catch (err) {
      console.error("Klaida:", err);
      toast.error("Nėra blokuotų rekomendacijų.");
    } finally {
      setUnblocking(null);
    }
  };

  useEffect(() => {
    if (userId) fetchData();
  }, [userId]);

  return (
    <div className="blokai-container">
      <ToastContainer />
      <h1 className="title">Blokuotos Rekomendacijos</h1>
      {loading ? (
        <div className="loading">Kraunama...</div>
      ) : error ? (
        <div className="error">{error}</div>
      ) : products.length === 0 ? (
        <div className="no-blocks-message">Nėra blokuotų rekomendacijų.</div>
      ) : (
        <table className="styled-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Pavadinimas</th>
              <th>Aprašymas</th>
              <th>Veiksmas</th>
            </tr>
          </thead>
          <tbody>
            {products.map((item, index) => (
              <tr key={item.id}>
                <td>{index + 1}</td>
                <td>{item.pavadinimas}</td>
                <td>{item.aprasymas}</td>
                <td>
                  <button
                    onClick={() => handleUnblockRecommendation(item.prekeId)}
                    className="action-button unblock-button"
                    disabled={unblocking === item.prekeId}
                  >
                    {unblocking === item.prekeId ? "Šalinama..." : "Atblokuoti"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default BlokuotosRekomendacijos;
