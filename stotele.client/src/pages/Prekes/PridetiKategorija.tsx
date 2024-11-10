import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const PridetiKategorija = () => {
  const [title, setTitle] = useState("");
  const navigate = useNavigate();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("New Category Title:", title);
    setTitle("");
    navigate("/prekiu-kategorijos");
  };

  return (
    <div>
      <h1>Pridėti Kategoriją</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Pavadinimas</label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite kategorijos pavadinimą"
          />
        </div>
        <button type="submit">Pridėti Kategoriją</button>
      </form>
    </div>
  );
};

export default PridetiKategorija;
