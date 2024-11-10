import React from "react";

const VisosKategorijos = () => {
  const categories = ["Technika", "Sodas ir daržas", "Statyba", "Dekoravimas"];

  return (
    <div>
      <h1>Visos Kategorijos</h1>
      <ul>
        {categories.map((category, index) => (
          <li key={index}>
            {category}
            <button style={{ marginLeft: "10px" }}>Pridėti prie mėgstamų</button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default VisosKategorijos;
