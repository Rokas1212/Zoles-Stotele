import React from "react";

const BlokuotosRekomendacijos = () => {
  const products = ["Žoliapjovė", "Laistymo žarna", "Trąšų maišas", "Sodo kastuvas"];

  return (
    <div>
      <h1>Blokuotos rekomendacijos</h1>
      <ul>
        {products.map((product, index) => (
          <li key={index}>
            {product}
            <button style={{ marginLeft: "10px" }}>Pašalinti</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default BlokuotosRekomendacijos;
