import React, { useState, useEffect } from "react";
import "./pridetinuolaida.css";

interface Preke {
  id: number;
  pavadinimas: string;
  kaina: number;
}

const PridetiNuolaida: React.FC = () => {
  const [amount, setAmount] = useState("");
  const [endDate, setEndDate] = useState("");
  const [prekes, setPrekes] = useState<Preke[]>([]);
  const [selectedPrekeId, setSelectedPrekeId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const today = new Date().toISOString().split("T")[0];

  // Fetch the list of available Prekes
  useEffect(() => {
    const fetchPrekes = async () => {
      try {
        const response = await fetch("https://localhost:5210/api/Preke");
        if (!response.ok) {
          throw new Error("Nepavyko gauti prekių sąrašo.");
        }
        const data: Preke[] = await response.json();
        setPrekes(data);
      } catch (err) {
        console.error("Klaida gaunant prekes:", err);
        setError("Nepavyko gauti prekių sąrašo. Bandykite dar kartą.");
      }
    };

    fetchPrekes();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!selectedPrekeId) {
      alert("Pasirinkite prekę.");
      return;
    }

    // Convert the end date to ISO format with UTC time
    const formattedEndDate = new Date(endDate).toISOString();

    const newDiscount = {
      procentai: parseInt(amount, 10),
      galiojimoPabaiga: formattedEndDate, // Send the properly formatted date
      prekeId: selectedPrekeId,
    };

    try {
      const response = await fetch("https://localhost:5210/api/Nuolaida", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(newDiscount),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Nepavyko pridėti nuolaidos.");
      }

      // Redirect to /nuolaidos after successful submission
      window.location.href = "/nuolaidos";
    } catch (err: any) {
      console.error("Klaida pridedant nuolaidą:", err);
      setError(err.message || "Nepavyko pridėti nuolaidos. Bandykite dar kartą.");
    }
  };

  return (
    <div className="form-container">
      <h1 className="form-title">Pridėk Savo Nuolaidą</h1>
      <form onSubmit={handleSubmit} className="discount-form">
        <div className="form-group">
          <label>Nuolaidos dydis (%)</label>
          <input
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            placeholder="Įveskite nuolaidos dydį"
            min="0"
            required
          />
        </div>
        <div className="form-group">
          <label>Galiojimo pabaiga</label>
          <input
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            min={today}
            required
          />
        </div>
        <div className="form-group">
          <label>Pasirinkite prekę</label>
          <select
            value={selectedPrekeId || ""}
            onChange={(e) => setSelectedPrekeId(parseInt(e.target.value))}
            required
          >
            <option value="" disabled>
              Pasirinkite prekę
            </option>
            {prekes.map((preke) => (
              <option key={preke.id} value={preke.id}>
                {preke.pavadinimas} - {preke.kaina.toFixed(2)} €
              </option>
            ))}
          </select>
        </div>
        <button type="submit" className="submit-button">Pridėti Nuolaidą</button>
      </form>

      {error && <p className="error-message">{error}</p>}
    </div>
  );
};

export default PridetiNuolaida;
