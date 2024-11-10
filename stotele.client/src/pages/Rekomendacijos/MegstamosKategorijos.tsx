import React from "react";

const MegstamosKategorijos = () => {
  const favoriteCategories = [
    { id: 1, kategorija: "Albaniška", dateAdded: "2024-11-03" },
    { id: 2, kategorija: "Natūrali", dateAdded: "2024-10-20" },
    { id: 3, kategorija: "Populiariausi", dateAdded: "2024-09-10" },
  ];

  return (
    <div>
      <h1>Mėgstamos kategorijos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Kategorija</th>
            <th>Pridėjimo data</th>
            <th>Veiksmas</th>
          </tr>
        </thead>
        <tbody>
          {favoriteCategories.map((item) => (
            <tr key={item.id}>
              <td>{item.id}</td>
              <td>{item.kategorija}</td>
              <td>{item.dateAdded}</td>
              <td>
                <button>Pašalinti</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default MegstamosKategorijos;
