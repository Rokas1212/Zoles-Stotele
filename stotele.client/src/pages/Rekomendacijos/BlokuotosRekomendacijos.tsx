import React from "react";

const BlokuotosRekomendacijos = () => {
  const blockedItems = [
    { id: 1, preke: "Albaniška žolė", dateAdded: "2024-11-01" },
    { id: 2, preke: "Gruziniška veja", dateAdded: "2024-10-25" },
    { id: 3, preke: "Žolytės suktinukai ALBANIA", dateAdded: "2024-09-15" },
  ];

  return (
    <div>
      <h1>Blokuotos rekomendacijos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Prekė</th>
            <th>Pridėjimo data</th>
          </tr>
        </thead>
        <tbody>
          {blockedItems.map((item) => (
            <tr key={item.id}>
              <td>{item.id}</td>
              <td>{item.preke}</td>
              <td>{item.dateAdded}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default BlokuotosRekomendacijos;
