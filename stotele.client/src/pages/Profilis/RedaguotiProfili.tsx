import React, { useState } from "react";

const RedaguotiProfili = () => {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Updated Profile:", { name, email, password });
  };

  return (
    <div>
      <h1>Redaguoti Profilį</h1>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Vardas</label>
          <input
            type="text"
            className="form-control"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Įveskite vardą"
          />
        </div>
        <div className="form-group">
          <label>El. paštas</label>
          <input
            type="email"
            className="form-control"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Įveskite el. paštą"
          />
        </div>
        <div className="form-group">
          <label>Slaptažodis</label>
          <input
            type="password"
            className="form-control"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Įveskite slaptažodį"
          />
        </div>
        <button type="submit" className="btn btn-primary mt-3">
          Išsaugoti
        </button>
      </form>
    </div>
  );
};

export default RedaguotiProfili;
