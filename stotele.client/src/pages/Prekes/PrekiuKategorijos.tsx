import React, { useState, useEffect } from "react";

type kategorijos = {
  id: number;
  pavadinimas: string;
  aprasymas: string;
  vadybininkasId: number;
  vadybininkas: string | null;
};

const PrekiuKategorijos = () => {
  const [categories, setCategories] = useState<kategorijos[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [admin, setAdmin] = useState<boolean>(false);

  useEffect(() => {
    const user = localStorage.getItem("user");

    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
      if (userObject.administratorius) setAdmin(true);
    } else {
      console.log("Naudotojas neprisijungęs");
    }

    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const response = await fetch(
        "https://localhost:5210/api/Kategorija/kategorijos"
      );
      const data = await response.json();
      setCategories(data);
    } catch (error) {
      console.error("Klaida gaunant prekes", error);
    }
  };

  const handleAddToFavorites = async (categoryId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      alert("Naudotojas neprisijungęs");
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/megstamos-kategorijos/prideti/${userId}/${categoryId}`,
        {
          method: "POST",
        }
      );

      if (response.ok) {
        alert("Prekė pridėta prie mėgstamų");
      }
      if (response.status === 400) {
        alert("Prekė jau yra pridėta prie mėgstamų");
      }
    } catch (error) {
      console.error("Klaida pridedant prekę prie mėgstamų", error);
    }
  };

  const handleRemoveCategory = async (categoryId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      alert("Naudotojas neprisijungęs");
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Kategorija/kategorija/istrinti/${userId}/${categoryId}`,
        {
          method: "DELETE",
        }
      );

      if (response.ok) {
        alert("Kategorija sėkmingai ištrinta");
      }
    } catch (error) {
      console.error("Klaida trinant kategoriją", error);
    }
  };

  return (
    <div>
      <h1>Prekių Kategorijos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Kategorijos pavadinimas</th>
            <th>Aprasymas</th>
            <th>Veiksmas</th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category, index) => (
            <tr key={category.id}>
              <td>{index + 1}</td>
              <td>{category.pavadinimas}</td>
              <td>{category.aprasymas}</td>
              <td>
                {!admin ? (
                  <button onClick={() => handleAddToFavorites(category.id)}>
                    Pridėti prie mėgstamų
                  </button>
                ) : (
                  <button onClick={() => handleRemoveCategory(category.id)}>
                    Pašalinti kategoriją
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PrekiuKategorijos;
