import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

const PridetiKategorija = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState(""); // New state for description
  const [userId, setUserId] = useState<number | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const user = localStorage.getItem("user");

    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
    } else {
      console.log("Naudotojas neprisijungęs");
    }
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const payload = {
      pavadinimas: title,
      aprasymas: description,
    };

    try {
      const response = await fetch(
        `https://localhost:5210/api/Kategorija/kategorija/prideti/${userId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(payload),
        }
      );

      if (response.ok) {
        alert("Kategorija pridėta");
        console.log("New Category Title:", title);
        console.log("Description:", description);
      }
      if (response.status === 400) {
        alert("Kategorija su tokiu pavadinimu jau egzistuoja");
      }
      if (response.status == 404) {
        alert("Naudotojas nėra vadybininkas");
      }
      setTitle("");
      setDescription(""); // Clear description after submit
      navigate("/prekiu-kategorijos");
    } catch (error) {
      console.error("Klaida pridedant kategoriją", error);
    }
  };

  return (
    <div className="container mt-5">
      <h1 className="text-center mb-4">Pridėti Kategoriją</h1>
      <form onSubmit={handleSubmit} className="card p-4 shadow-sm">
        <div className="mb-3">
          <label htmlFor="title" className="form-label">
            Pavadinimas
          </label>
          <input
            type="text"
            id="title"
            className="form-control"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite kategorijos pavadinimą"
          />
        </div>

        {/* Description input field */}
        <div className="mb-3">
          <label htmlFor="description" className="form-label">
            Aprašymas
          </label>
          <textarea
            id="description"
            className="form-control"
            rows={3}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Įveskite kategorijos aprašymą"
          />
        </div>

        <button type="submit" className="btn btn-primary w-100">
          Pridėti Kategoriją
        </button>
      </form>
    </div>
  );
};

export default PridetiKategorija;
