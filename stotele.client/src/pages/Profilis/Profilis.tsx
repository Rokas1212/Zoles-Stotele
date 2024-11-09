import React from "react";

const Profilis = () => {
  return (
    <div>
      <h1 className="display-4">Profilis</h1>
      <ul className="list-group">
        <li className="list-group-item">
          <a href="/uzsakymai" className="nav-link">
            Užsakymai
          </a>
        </li>
        <li className="list-group-item">
          <a href="/" className="nav-link">
            Blokuotos Rekomendacijos
          </a>
        </li>
        <li className="list-group-item">
          <a href="/" className="nav-link">
            Mėgstamos kategorijos
          </a>
        </li>
        <li className="list-group-item">
          <a href="/" className="nav-link">
            Redaguoti profilį
          </a>
        </li>
      </ul>
    </div>
  );
};

export default Profilis;
