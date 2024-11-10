import React from "react";

const Prekes = () => {
  const products = [
    { id: "1", name: "Albaniška žolė", price: "€10.00" },
    { id: "2", name: "Žolės stotelės firminė žolė", price: "€15.00" },
    { id: "3", name: "Piktžolių veja", price: "€20.00" },
  ];

  return (
    <div>
      <h1>Prekės</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Pavadinimas</th>
            <th>Kaina</th>
            <th>Veiksmai</th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td>
                <a href={`/preke?id=${product.id}`}>{product.id}</a>
              </td>
              <td>{product.name}</td>
              <td>{product.price}</td>
              <td>
                <a href={`/preke?id=${product.id}`}>Peržiūrėti</a>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Prekes;
