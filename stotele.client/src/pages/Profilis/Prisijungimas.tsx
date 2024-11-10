import React, { useState } from "react";

const PrisijungimoLangas = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Logging in with:", username, password);
    window.location.href = "/";
  };

  return (
    <div>
      <h1>Prisijungimas</h1>
      <form onSubmit={handleLogin}>
        <div>
          <label>Naudotojo vardas:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Slaptažodis:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit">Prisijungti</button>
      </form>
      <a href="/registracija">Registracija</a>
    </div>
  );
};

export default PrisijungimoLangas;
