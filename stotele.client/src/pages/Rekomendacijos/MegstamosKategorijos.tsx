import React, { useState, useEffect } from "react";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./blokuotosRekomendacijos.css";

type Product = {
  id: number;
  pridejimoData: string;
  kategorijaId: number;
  kategorijaPavadinimas: string;
  kategorijaAprasymas: string;
};

const MegstamosKategorijos: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState<number | null>(null);

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
        `/api/Rekomendacija/megstamos-kategorijos/${userId}`
      );
      if (!response.ok) throw new Error("Sąrašas tuščias.");

      const data: Product[] = await response.json();
      setProducts(data.sort((a, b) => a.id - b.id));
    } catch (err: any) {
      console.error("Klaida:", err);
      setError(err.message || "Sąrašas tuščias.");
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteCategory = async (categoryId: number) => {
    const isConfirmed = window.confirm(
      "Ar tikrai norite pašalinti šią kategoriją?"
    );

    if (!isConfirmed) return;

    setDeleting(categoryId);
    try {
      const response = await fetch(
        `/api/Rekomendacija/megstamos-kategorijos/istrinti/${userId}/${categoryId}`,
        { method: "DELETE" }
      );

      if (!response.ok) throw new Error("Nepavyko pašalinti kategorijos.");

      toast.success("Kategorija sėkmingai pašalinta!", {
        position: "top-right",
        autoClose: 2000,
      });

      fetchData();
    } catch (err) {
      console.error("Klaida:", err);
      toast.error("Nepavyko pašalinti kategorijos.");
    } finally {
      setDeleting(null);
    }
  };

  useEffect(() => {
    if (userId) fetchData();
  }, [userId]);

  return (
    <div className="blokai-container">
      <ToastContainer />
      <h1 className="title">Mėgstamos Kategorijos</h1>
      {loading ? (
        <div className="loading">Kraunama...</div>
      ) : error ? (
        <div className="error">{error}</div>
      ) : products.length === 0 ? (
        <div className="no-favorites-message">Nėra mėgstamų kategorijų.</div>
      ) : (
        <table className="styled-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Kategorija</th>
              <th>Aprašymas</th>
              <th>Veiksmas</th>
            </tr>
          </thead>
          <tbody>
            {products.map((item, index) => (
              <tr key={item.id}>
                <td>{index + 1}</td>
                <td>{item.kategorijaPavadinimas}</td>
                <td>{item.kategorijaAprasymas}</td>
                <td>
                  <button
                    onClick={() => handleDeleteCategory(item.kategorijaId)}
                    className="action-button unblock-button"
                    disabled={deleting === item.kategorijaId}
                  >
                    {deleting === item.kategorijaId
                      ? "Šalinama..."
                      : "Pašalinti"}
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

export default MegstamosKategorijos;
