import React, { useState } from "react";

const PridetiPreke = () => {
  const [title, setTitle] = useState("");
  const [price, setPrice] = useState("");
  const [expireDate, setExpireDate] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Nauja Prekė:", { title, price, expireDate });
  };

  return (
    <div>
      <h1>Pridėti Prekę</h1>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Pavadinimas</label>
          <input
            type="text"
            className="form-control"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite pavadinimą"
          />
        </div>
        <div className="form-group">
          <label>Kaina</label>
          <input
            type="text"
            className="form-control"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            placeholder="Įveskite kainą"
          />
        </div>
        <div className="form-group">
          <label>Galiojimo data</label>
          <input
            type="date"
            className="form-control"
            value={expireDate}
            onChange={(e) => setExpireDate(e.target.value)}
          />
        </div>
        <button
        onClick={() => (window.location.href = "/preke?id=1")}
        className="btn btn-primary mt-3"
      >
        Pridėti
      </button>
      </form>

      
    </div>
  );
};

export default PridetiPreke;
