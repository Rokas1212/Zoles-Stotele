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
          <a href="/blokuotos-rekomendacijos" className="nav-link">
            Blokuotos Rekomendacijos
          </a>
        </li>
        <li className="list-group-item">
          <a href="/megstamos-kategorijos" className="nav-link">
            Mėgstamos kategorijos
          </a>
        </li>
        <li className="list-group-item">
          <a href="/redaguoti-profili" className="nav-link">
            Redaguoti profilį
          </a>
        </li>
        <li className="list-group-item">
          <button className="btn btn-warning">Pridėti taškų</button>
        </li>
      </ul>
    </div>
  );
};

export default Profilis;
