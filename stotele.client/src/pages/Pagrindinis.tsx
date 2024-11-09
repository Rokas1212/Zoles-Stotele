const Pagrindinis = () => {
  return (
    <div>
      <h1>Pagrindinis</h1>
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
        <li className="list-group-item">
          <a href="/krepselis" className="nav-link">
            Krepšelis
          </a>
        </li>
        <li className="list-group-item">
          <a href="/nuolaidos" className="nav-link">
            Nuolaidos
          </a>
        </li>
      </ul>
    </div>
  );
};

export default Pagrindinis;
