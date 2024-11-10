import React from "react";

const Preke = () => {
  const searchParams = new URLSearchParams(window.location.search);
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

  return (
    <div>
      <h1>{product.name}</h1>
      <p>Kaina: {product.price}</p>
      <a href={`/redaguoti-preke?id=${product.id}`}>
        <button>Redaguoti</button>
      </a>
    </div>
  );
};

export default Preke;
