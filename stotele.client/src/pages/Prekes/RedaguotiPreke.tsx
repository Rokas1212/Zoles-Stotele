import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";

const products = [
  { id: "1", name: "Albaniška žolė", price: "€10.00", description: "Popular Albanian grass" },
  { id: "2", name: "Žolės stotelės firminė žolė", price: "€15.00", description: "Premium grass from Žolės Stotelė" },
  { id: "3", name: "Piktžolių veja", price: "€20.00", description: "Grass for a weedy lawn" },
];

const RedaguotiPreke = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");

  const product = products.find((p) => p.id === id);

  const [title, setTitle] = useState(product ? product.name : "");
  const [price, setPrice] = useState(product ? product.price : "");
  const [description, setDescription] = useState(product ? product.description : "");

  useEffect(() => {
    if (product) {
      setTitle(product.name);
      setPrice(product.price);
      setDescription(product.description);
    }
  }, [product]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Updated Product:", { title, price, description });

    navigate(`/preke?id=${id}`);
  };

  if (!product) {
    return <div>Prekė nerasta</div>;
  }

  return (
    <div>
      <h1>Redaguoti Prekę</h1>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Pavadinimas</label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite prekės pavadinimą"
          />
        </div>
        <div>
          <label>Kaina</label>
          <input
            type="text"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            placeholder="Įveskite prekės kainą"
          />
        </div>
        <div>
          <label>Aprašymas</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Įveskite prekės aprašymą"
          />
        </div>
        <button type="submit">Išsaugoti</button>
      </form>
    </div>
  );
};

export default RedaguotiPreke;
