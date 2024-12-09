import React, { useEffect, useState } from "react";

interface Discount {
  id: number;
  procentai: number;
  pabaigosData: string;
  uzsakymasId: number | null;
  prekeId: number | null;
  uzsakymas: {
    id: number;
    data: string;
    suma: number;
  } | null;
  preke: {
    id: number;
    kaina: number;
    pavadinimas: string;
    kodas: number;
    galiojimoData: string;
  } | null;
}

const Nuolaidos: React.FC = () => {
  const [discounts, setDiscounts] = useState<Discount[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchDiscounts = async () => {
      try {
        const response = await fetch("http://localhost:5210/api/Nuolaida");
        if (!response.ok) {
          throw new Error(`Failed to fetch discounts: ${response.status} - ${response.statusText}`);
        }
        const data: Discount[] = await response.json();
        setDiscounts(data);
      } catch (err: any) {
        console.error('Error fetching discounts:', err);
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
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <h1>Nuolaidos</h1>
      <table className="table table-striped">
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
              <td>{new Date(discount.pabaigosData).toLocaleDateString()}</td>
              <td>{discount.preke?.pavadinimas || "N/A"}</td>
              <td>
                <a href={`/nuolaida?id=${discount.id}`}>Peržiūrėti</a>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <button onClick={handleAddDiscount}>Pridėti nuolaidą</button>
    </div>
  );
};

export default Nuolaidos;