import React, { useState, useEffect } from "react";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./pridetinuolaida.css";

interface Preke {
  id: number;
  pavadinimas: string;
  kaina: number;
}

interface Nuolaida {
  prekeId: number;
  galiojimoPabaiga: string;
}

const PridetiNuolaida: React.FC = () => {
  const [amount, setAmount] = useState("");
  const [endDate, setEndDate] = useState("");
  const [prekes, setPrekes] = useState<Preke[]>([]);
  const [nuolaidos, setNuolaidos] = useState<Nuolaida[]>([]);
  const [selectedPrekeId, setSelectedPrekeId] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const today = new Date().toISOString().split("T")[0];

  useEffect(() => {
    const fetchPrekesAndNuolaidos = async () => {
      try {
        const [prekesResponse, nuolaidosResponse] = await Promise.all([
          fetch("/api/Preke"),
          fetch("/api/Nuolaida/active"),
        ]);

        if (!prekesResponse.ok || !nuolaidosResponse.ok) {
          throw new Error("Nepavyko gauti duomenų.");
        }

        const prekesData: Preke[] = await prekesResponse.json();
        const nuolaidosData: Nuolaida[] = await nuolaidosResponse.json();

        setPrekes(prekesData);
        setNuolaidos(nuolaidosData);
      } catch (err) {
        console.error("Klaida gaunant duomenis:", err);
        setError("Nepavyko gauti duomenų. Bandykite dar kartą.");
      }
    };

    fetchPrekesAndNuolaidos();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!selectedPrekeId) {
      toast.error("Pasirinkite prekę.");
      return;
    }

    const discountValue = parseInt(amount, 10);
    if (discountValue < 0 || discountValue > 100) {
      setError("Nuolaidos dydis turi būti tarp 0 ir 100.");
      return;
    }

    const formattedEndDate = new Date(endDate).toISOString();

    const newDiscount = {
      procentai: discountValue,
      galiojimoPabaiga: formattedEndDate,
      prekeId: selectedPrekeId,
    };

    try {
      const response = await fetch("/api/Nuolaida", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newDiscount),
      });

      if (!response.ok) throw new Error("Nepavyko pridėti nuolaidos.");

      toast.success("Nuolaida sėkmingai pridėta!");

      setTimeout(() => {
        window.location.href = "/nuolaidos";
      }, 1500);
    } catch (err: any) {
      toast.error(err.message || "Nepavyko pridėti nuolaidos.");
    }
  };

  const filteredPrekes = prekes.filter(
    (preke) => !nuolaidos.some((nuolaida) => nuolaida.prekeId === preke.id)
  );

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
            onChange={(e) => setAmount(e.target.value)}
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
            {filteredPrekes.map((preke) => (
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
