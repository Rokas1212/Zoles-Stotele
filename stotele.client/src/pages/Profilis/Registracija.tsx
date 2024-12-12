import React, { useState } from "react";
import axios from "axios";
import "./registracija.css";

const RegistracijosLangas = () => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [gender, setGender] = useState("Vyras");
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const handleRegistration = async (e: React.FormEvent) => {
    e.preventDefault();

    const registrationData = {
      lytis: gender,
      elektroninisPastas: email,
      slaptazodis: password,
      vardas: firstName,
      slapyvardis: username,
      pavarde: lastName,
    };

    try {
      await axios.post("https://localhost:5210/api/Profilis/register", registrationData);

      setSuccessMessage("Registracija sėkminga! Jūs galite prisijungti.");
      setErrorMessage(""); 

      setUsername("");
      setEmail("");
      setPassword("");
      setFirstName("");
      setLastName("");
      setGender("Vyras");

      setTimeout(() => {
        window.location.href = "/";
      }, 500);
    } catch (error: any) {
      setSuccessMessage("");
      setErrorMessage(
        "Registracija nepavyko: " +
          (error.response?.data || error.message || "Klaida")
      );
    }
  };

  return (
    <div className="registration-container">
      <h1 className="registration-header">Registracija</h1>
      {successMessage && <div className="success-message">{successMessage}</div>}
      {errorMessage && <div className="error-message">{errorMessage}</div>}
      <form onSubmit={handleRegistration} className="registration-form">
        <div className="form-group">
          <label>Naudotojo vardas:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>El. paštas:</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>Slaptažodis:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>Vardas:</label>
          <input
            type="text"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>Pavardė:</label>
          <input
            type="text"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>Lytis:</label>
          <select value={gender} onChange={(e) => setGender(e.target.value)}>
            <option value="Vyras">Vyras</option>
            <option value="Moteris">Moteris</option>
            <option value="Kita">Kita</option>
          </select>
        </div>
        <button type="submit" className="registration-button">
          Registruotis
        </button>
      </form>
    </div>
  );
};

export default RegistracijosLangas;
