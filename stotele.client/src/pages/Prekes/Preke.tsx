import React from "react";
import { useLocation, useNavigate } from "react-router-dom";

const Preke = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");

  const products = [
    { id: "1", name: "Albaniška žolė", price: "€10.00" },
    { id: "2", name: "Žolės stotelės firminė žolė", price: "€15.00" },
    { id: "3", name: "Piktžolių veja", price: "€20.00" },
  ];

  const product = products.find((p) => p.id === id);

  if (!product) {
    return <div>Prekė nerasta</div>;
  }

  const handleEdit = () => {
    navigate(`/redaguoti-preke?id=${product.id}`);
  };

  return (
    <div>
      <h1>{product.name}</h1>
      <p>Kaina: {product.price}</p>
      <button onClick={handleEdit}>Redaguoti</button>
    </div>
  );
};

export default Preke;
