import React, { useState, useEffect } from "react";
import axios from "axios";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./redaguotiProfili.css";

const RedaguotiProfili = () => {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [gender, setGender] = useState("Vyras"); // Default gender
  const [city, setCity] = useState("");
  const [address, setAddress] = useState("");
  const [postalCode, setPostalCode] = useState("");
  const [birthDate, setBirthDate] = useState("");
  const [error, setError] = useState<string | null>(null);

  const userId = JSON.parse(localStorage.getItem("user") || "{}").id;

  useEffect(() => {
    axios
      .get(`/api/Profilis/${userId}`)
      .then((response) => {
        const data = response.data;
        setName(data.vardas || "");
        setEmail(data.elektroninisPastas || "");
        setGender(data.lytis || "Vyras");
        setCity(data.klientas?.miestas || "");
        setAddress(data.klientas?.adresas || "");
        setPostalCode(data.klientas?.pastoKodas || "");
        setBirthDate(data.klientas?.gimimoData || "");
      })
      .catch(() => setError("Nepavyko gauti naudotojo duomenų."));
  }, [userId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const payload: any = {};
    if (name.trim()) payload.vardas = name.trim();
    if (email.trim()) payload.elektroninisPastas = email.trim();
    if (password.trim()) payload.slaptazodis = password.trim();
    if (gender.trim()) payload.lytis = gender.trim();

    const klientas: any = {};
    if (city.trim()) klientas.miestas = city.trim();
    if (address.trim()) klientas.adresas = address.trim();
    if (postalCode) klientas.pastoKodas = parseInt(postalCode);
    if (birthDate.trim()) klientas.gimimoData = birthDate;

    if (Object.keys(klientas).length > 0) payload.klientas = klientas;

    try {
      await axios.put(`/api/Profilis/${userId}`, payload, {
        headers: { "Content-Type": "application/json" },
      });

      toast.success("Profilis sėkmingai atnaujintas!");
      setTimeout(() => {
        window.location.href = `/profilis?id=${userId}`;
      }, 2000);
    } catch (error) {
      setError("Nepavyko atnaujinti profilio. Bandykite dar kartą.");
      toast.error("Nepavyko atnaujinti profilio. Bandykite dar kartą.");
    }
  };

  return (
    <div className="edit-profile-container">
      <h1>Redaguoti Profilį</h1>
      <ToastContainer />
      {error && <div className="error">{error}</div>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <input
            type="text"
            placeholder="Vardas"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <div className="form-group">
          <input
            type="email"
            placeholder="El. paštas"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <div className="form-group">
          <input
            type="password"
            placeholder="Slaptažodis"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <div className="form-group">
          <select
            value={gender}
            onChange={(e) => setGender(e.target.value)}
            className="form-control modern-input"
          >
            <option value="Vyras">Vyras</option>
            <option value="Moteris">Moteris</option>
            <option value="Kita">Kita</option>
          </select>
        </div>
        <div className="form-group">
          <input
            type="text"
            placeholder="Miestas"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <div className="form-group">
          <input
            type="text"
            placeholder="Adresas"
            value={address}
            onChange={(e) => setAddress(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <div className="form-group">
          <input
            type="text"
            placeholder="Pašto Kodas"
            value={postalCode}
            onChange={(e) => setPostalCode(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <div className="form-group">
          <input
            type="date"
            placeholder="Gimimo Data"
            value={birthDate}
            onChange={(e) => setBirthDate(e.target.value)}
            className="form-control modern-input"
          />
        </div>
        <button type="submit" className="btn-success modern-button">
          Išsaugoti
        </button>
      </form>
    </div>
  );
};

export default RedaguotiProfili;
