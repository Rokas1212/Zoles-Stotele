import React from "react";

const Profiliai = () => {
  return (
    <div>
      <h1 className="display-4">Naudotojų profiliai</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Naudotojo ID</th>
            <th>Vardas</th>
            <th>Pavardė</th>
            <th>El. paštas</th>
            <th>Veiksmai</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>1</td>
            <td>Jonas</td>
            <td>Jonaitis</td>
            <td>joncee@gmail.com</td>
            <td>
              <a href="/profilis?id=1">Peržiūrėti</a>
            </td>
          </tr>
          <tr>
            <td>2</td>
            <td>Ona</td>
            <td>Onaitė</td>
            <td>poniaona@siksiksik.com</td>
            <td>
              <a href="/profilis?id=2">Peržiūrėti</a>
            </td>
          </tr>
          <tr>
            <td>3</td>
            <td>Petras</td>
            <td>Dovydaitis</td>
            <td>petras@garaziniai.lt</td>
            <td>
              <a href="/profilis?id=3">Peržiūrėti</a>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  );
};

export default Profiliai;
