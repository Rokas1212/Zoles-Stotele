import React, { useState, useEffect } from "react";

// Adjust this interface to match your actual Nuolaida model fields
interface INuolaida {
  id: number;
  procentai: number;
  pabaigosData: string; // Assuming your API returns ISO date strings
  uzsakymas?: {
    id: number;
    data: string;
    suma: number;
  };
  preke?: {
    id: number;
    kaina: number;
    pavadinimas: string;
    kodas: number;
    galiojimoData: string;
  };
}

const Nuolaida: React.FC = () => {
  const searchParams = new URLSearchParams(window.location.search); // Get query parameters
  const id = searchParams.get("id"); // Extract the "id" parameter

  const [nuolaida, setNuolaida] = useState<INuolaida | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setError("Nuolaida ID is missing from the query parameters.");
      setLoading(false);
      return;
    }

    const fetchNuolaida = async () => {
      try {
        const response = await fetch(`http://localhost:5210/api/Nuolaida/${id}`);
        if (!response.ok) {
          throw new Error(`Failed to fetch nuolaida: ${response.status} - ${response.statusText}`);
        }

        const data: INuolaida = await response.json();
        setNuolaida(data);
      } catch (err: any) {
        console.error("Error fetching nuolaida:", err);
        setError(err.message || "An unknown error occurred");
      } finally {
        setLoading(false);
      }
    };

    fetchNuolaida();
  }, [id]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  if (!nuolaida) {
    return <div>Nuolaida not found</div>;
  }

  return (
    <div>
      <h1>Nuolaida</h1>
      <p><strong>Nuolaidos kodas:</strong> {nuolaida.id}</p>
      <p><strong>Nuolaidos dydis (%):</strong> {nuolaida.procentai}%</p>
      <p><strong>Galiojimo pabaiga:</strong> {new Date(nuolaida.pabaigosData).toLocaleDateString()}</p>
      {nuolaida.preke && (
        <div>
          <p><strong>Prekės pavadinimas:</strong> {nuolaida.preke.pavadinimas}</p>
          <p><strong>Prekės kaina:</strong> {nuolaida.preke.kaina}</p>
        </div>
      )}
      {nuolaida.uzsakymas && (
        <div>
          <p><strong>Užsakymo suma:</strong> {nuolaida.uzsakymas.suma}</p>
          <p><strong>Užsakymo data:</strong> {new Date(nuolaida.uzsakymas.data).toLocaleDateString()}</p>
        </div>
      )}
      <button onClick={() => alert("Delete functionality not implemented yet")}>
        Ištrinti nuolaidą
      </button>
    </div>
  );
};

export default Nuolaida;
