const Pagrindinis = () => {
  const recommendedProducts = [
    { name: "Žoliapjovė", imgSrc: "https://via.placeholder.com/100" },
    { name: "Laistymo žarna", imgSrc: "https://via.placeholder.com/100" },
    { name: "Trąšų maišas", imgSrc: "https://via.placeholder.com/100" },
    { name: "Sodo kastuvas", imgSrc: "https://via.placeholder.com/100" },
  ];

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

      <h2>Rekomenduojamos prekės</h2>
      <div>
        {recommendedProducts.map((product, index) => (
          <div key={index} style={{ display: "flex", alignItems: "center", marginBottom: "10px" }}>
            <img src={product.imgSrc} alt={product.name} width="100" height="100" />
            <div style={{ marginLeft: "10px" }}>
              <p>{product.name}</p>
              <button style={{ marginTop: "5px" }}>Blokuoti rekomendaciją</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Pagrindinis;
