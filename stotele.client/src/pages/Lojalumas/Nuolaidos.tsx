const Nuolaidos = () => {
  return (
    <div>
      <h1>Nuolaidos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Nuolaidos kodas</th>
            <th>Nuolaidos pavadinimas</th>
            <th>Nuolaidos dydis</th>
            <th>Galiojimo pradzia</th>
            <th>Galiojimo pabaiga</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>
              <a href="/nuolaida?id=1">1</a>
            </td>
            <td>nuolaida1</td>
            <td>10</td>
            <td>2021-10-10</td>
            <td>2021-10-20</td>
          </tr>
          <tr>
            <td>
              <a href="/nuolaida?id=2">2</a>
            </td>
            <td>nuolaida2</td>
            <td>20</td>
            <td>2021-10-11</td>
            <td>2021-10-21</td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default Nuolaidos;
