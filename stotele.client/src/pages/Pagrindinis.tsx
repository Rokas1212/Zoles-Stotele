const Pagrindinis = () => {
  return (
    <div>
      <h1>Pagrindinis</h1>
      <ul className="list-group">
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
        <li className="list-group-item">
          <a href="/prekes" className="nav-link">
            Prekės
          </a>
        </li>
        <li className="list-group-item">
          <a href="/prekiu-kategorijos" className="nav-link">
            Visos kategorijos
          </a>
        </li>
      </ul>
    </div>
  );
};

export default Pagrindinis;
