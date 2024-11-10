import React from "react";

const categories = [
  { id: "1", name: "Žolės sėklos", icon: "🌱" },
  { id: "2", name: "Vežanti žolė", icon: "🚜" },
  { id: "3", name: "Firminė žolė", icon: "🧪" },
  { id: "4", name: "Vandens pavidalo žolė", icon: "💧" },
  { id: "5", name: "Žolė geram vakarui", icon: "🤵" },
  { id: "6", name: "Magiška žolė", icon: "🎩" },
];

const PrekiuKategorijos = () => {
  return (
    <div>
      <h1>Prekių Kategorijos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Kategorijos pavadinimas</th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.id}>
              <td>{category.id}</td>
              <td>
                {category.icon} {category.name}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PrekiuKategorijos;
