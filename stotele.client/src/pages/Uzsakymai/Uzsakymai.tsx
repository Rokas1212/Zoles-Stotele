const Uzsakymai = () => {
  return (
    <div>
      <h1>Uzsakymai</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Uzsakymo numeris</th>
            <th>Uzsakymo data</th>
            <th>Uzsakymo busena</th>
            <th>Uzsakymo suma</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>
              <a href="/uzsakymas?id=1">1</a>
            </td>
            <td>2021-10-10</td>
            <td>Patvirtintas</td>
            <td>100</td>
          </tr>
          <tr>
            <td>
              <a href="/uzsakymas?id=2">2</a>
            </td>
            <td>2021-10-11</td>
            <td>lol nzn</td>
            <td>200</td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default Uzsakymai;
