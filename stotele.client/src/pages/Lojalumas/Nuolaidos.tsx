import React, { useEffect, useState } from "react";
import Loading from "../../components/loading";
import "./nuolaidos.css"; // Create this file for styles

interface Discount {
  id: number;
  procentai: number;
  galiojimoPabaiga: string;
  prekesPavadinimas: string;
}

const Nuolaidos: React.FC = () => {
  const [discounts, setDiscounts] = useState<Discount[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchDiscounts = async () => {
      try {
        const response = await fetch("https://localhost:5210/api/Nuolaida");
        if (!response.ok) {
          throw new Error(
            `Failed to fetch discounts: ${response.status} - ${response.statusText}`
          );
        }
        const data: Discount[] = await response.json();

        const formattedData = data.map((discount) => ({
          ...discount,
          galiojimoPabaiga: new Date(discount.galiojimoPabaiga).toISOString(),
        }));

        setDiscounts(formattedData);
      } catch (err: any) {
        console.error("Error fetching discounts:", err);
        setError(err.message || "An unknown error occurred");
      } finally {
        setLoading(false);
      }
    };
    fetchDiscounts();
  }, []);

  const handleAddDiscount = () => {
    window.location.href = "/administratorius/prideti-nuolaida";
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="error-message">Error: {error}</div>;
  }

  return (
    <div className="nuolaidos-container">
      <h1 className="title">Nuolaidos</h1>
      <table className="table">
  <thead>
    <tr>
      <th>Nuolaidos kodas</th>
      <th>Nuolaidos dydis (%)</th>
      <th>Galiojimo pabaiga</th>
      <th>Prekės pavadinimas</th>
      <th>Veiksmai</th>
    </tr>
  </thead>
  <tbody>
    {discounts.map((discount) => (
      <tr key={discount.id}>
        <td>
          <a href={`/nuolaida?id=${discount.id}`}>{discount.id}</a>
        </td>
        <td>{discount.procentai}</td>
        <td>{new Date(discount.galiojimoPabaiga).toLocaleDateString()}</td>
        <td>{discount.prekesPavadinimas || "N/A"}</td>
        <td>
          <a href={`/nuolaida?id=${discount.id}`}>Peržiūrėti</a>
        </td>
      </tr>
    ))}
  </tbody>
</table>

      <button className="add-discount-button" onClick={handleAddDiscount}>
        Pridėti nuolaidą
      </button>
    </div>
  );
};

export default Nuolaidos;
