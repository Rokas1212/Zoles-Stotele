import React from "react";

const discounts = [
  { id: "1", name: "Žolės šventės nuolaida", amount: 10, startDate: "2021-10-10", endDate: "2021-10-20" },
  { id: "2", name: "Black friday", amount: 20, startDate: "2021-11-11", endDate: "2021-11-31" },
];

const Nuolaidos = () => {
  const handleAddDiscount = () => {
    window.location.href = "/administratorius/prideti-nuolaida";
  };

  return (
    <div>
      <h1>Nuolaidos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Nuolaidos kodas</th>
            <th>Nuolaidos pavadinimas</th>
            <th>Nuolaidos dydis (%)</th>
            <th>Galiojimo pradžia</th>
            <th>Galiojimo pabaiga</th>
            <th>Veiksmai</th>
          </tr>
        </thead>
        <tbody>
          {discounts.map((discount) => (
            <tr key={discount.id}>
              <td>
                <a href={`/nuolaida?id=${discount.id}`}>{discount.id}</a>
              </td>
              <td>{discount.name}</td>
              <td>{discount.amount}</td>
              <td>{discount.startDate}</td>
              <td>{discount.endDate}</td>
              <td>
                <a href={`/nuolaida?id=${discount.id}`}>Peržiūrėti</a>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <button onClick={handleAddDiscount}>Pridėti nuolaidą</button>
    </div>
  );
};

export default Nuolaidos;
