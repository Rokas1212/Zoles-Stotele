import React, { useState } from "react";

const PridetiNuolaida = () => {
  const [title, setTitle] = useState("");
  const [amount, setAmount] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setTitle("");
    setAmount("");
    setStartDate("");
    setEndDate("");
    setSubmitted(true);
  };

  return (
    <div>
      <h1>Pridek Tu Ta Savo Nuolaida Genijau</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Pavadinimas</label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite nuolaidos pavadinimą"
          />
        </div>
        <div>
          <label>Nuolaidos dydis (%)</label>
          <input
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            placeholder="Įveskite nuolaidos dydį"
          />
        </div>
        <div>
          <label>Galiojimo pradžia</label>
          <input
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>
        <div>
          <label>Galiojimo pabaiga</label>
          <input
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
        </div>
        <button type="submit">Pridėti Nuolaidą</button>
      </form>

      {submitted && (
        <p>
          Nuolaida pridėta! <a href="/nuolaidos">Grįžti į Nuolaidų sąrašą</a>
        </p>
      )}
    </div>
  );
};

export default PridetiNuolaida;
