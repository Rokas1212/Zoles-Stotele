import React, { useState, useEffect } from "react";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
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

    const discountValue = parseInt(amount, 10);

    if (discountValue < 0 || discountValue > 100) {
      setError("Nuolaidos dydis turi būti tarp 0 ir 100.");
      return;
    }

    if (!selectedPrekeId) {
      toast.error("Pasirinkite prekę.");
      return;
    }

    const formattedEndDate = new Date(endDate).toISOString();

    const newDiscount = {
      procentai: discountValue,
      galiojimoPabaiga: formattedEndDate, 
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

      // Show success notification
      toast.success("Nuolaida sėkmingai pridėta!");

      // Redirect to /nuolaidos after a short delay
      setTimeout(() => {
        window.location.href = "/nuolaidos";
      }, 1500);
    } catch (err: any) {
      console.error("Klaida pridedant nuolaidą:", err);
      setError(err.message || "Nepavyko pridėti nuolaidos. Bandykite dar kartą.");
      toast.error(err.message || "Nepavyko pridėti nuolaidos.");
    }
  };

  return (
    <div className="form-container">
      <ToastContainer /> 
      <h1 className="form-title">Pridėk Savo Nuolaidą</h1>
      <form onSubmit={handleSubmit} className="discount-form">
        <div className="form-group">
          <label>Nuolaidos dydis (%)</label>
          <input
            type="number"
            value={amount}
            onChange={(e) => {
              const value = e.target.value;
              if (+value >= 0 && +value <= 100) {
                setAmount(value);
                setError(null);
              } else {
                setError("Nuolaidos dydis turi būti tarp 0 ir 100.");
              }
            }}
            placeholder="Įveskite nuolaidos dydį"
            min="0"
            max="100"
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
        <button type="submit" className="submit-button">
          Pridėti Nuolaidą
        </button>
      </form>

      {error && <p className="error-message">{error}</p>}
    </div>
  );
};

export default PridetiNuolaida;
