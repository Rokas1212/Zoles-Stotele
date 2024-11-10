import React, { useState } from "react";

const RegistracijosLangas = () => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleRegistration = (e: React.FormEvent) => {
    e.preventDefault();
    // Add registration logic here
    console.log("Registering with:", username, email, password);
    // redirect to pagrindinis
    window.location.href = "/";
  };

  return (
    <div>
      <h1>Registracija</h1>
      <form onSubmit={handleRegistration}>
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
          <label>El. paštas:</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
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
        <button type="submit">Registruotis</button>
      </form>
    </div>
  );
};

export default RegistracijosLangas;
