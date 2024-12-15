import React, { useState, useEffect } from "react";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./prekiuKategorijos.css";

type kategorijos = {
  id: number;
  pavadinimas: string;
  aprasymas: string;
  vadybininkasId: number;
  vadybininkas: string | null;
};

const PrekiuKategorijos: React.FC = () => {
  const [categories, setCategories] = useState<kategorijos[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [admin, setAdmin] = useState<boolean>(false);

  const fetchUserCheck = async () => {
    try {
      const response = await fetch(
        `https://localhost:5210/api/Profilis/is-vadybininkas/${userId}`
      );
      if (response.ok) {
        setAdmin(true);
      }
    } catch (error) {
      console.error("Klaida gaunant vadybininko informaciją", error);
    }
  };

  useEffect(() => {
    const user = localStorage.getItem("user");

    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
      console.log(userId);
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
      console.error("Klaida gaunant kategorijas", error);
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
        alert("Kategorija pridėta prie mėgstamų");
      }
      if (response.status === 400) {
        alert("Kategorija jau yra pridėta prie mėgstamų");
      }
    } catch (error) {
      console.error("Klaida pridedant Kategorija prie mėgstamų", error);
    }
  };

  const handleRemoveCategory = async (categoryId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      alert("Naudotojas neprisijungęs");
      return;
    }

    const isConfirmed = window.confirm(
      "Ar tikrai norite pašalinti šią kategoriją?"
    );

    if (!isConfirmed) return;

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

      await fetchData();
    } catch (error) {
      console.error("Klaida trinant kategoriją", error);
    }
  };

  useEffect(() => {
    if (userId !== null) {
      fetchUserCheck();
    }
  }, [userId]);

  return (
    <div className="kategorijos-container">
      <h1 className="kategorijos-title">Prekių Kategorijos</h1>
      <table className="kategorijos-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Kategorijos pavadinimas</th>
            <th>Aprašymas</th>
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
                  <button
                    className="action-button add-to-favorites"
                    onClick={() => handleAddToFavorites(category.id)}
                  >
                    Pridėti prie mėgstamų
                  </button>
                ) : (
                  <>
                    <button
                      className="action-button"
                      onClick={() => handleRemoveCategory(category.id)}
                    >
                      Pašalinti kategoriją
                    </button>
                    <button
                      className="action-button add-to-favorites"
                      onClick={() => handleAddToFavorites(category.id)}
                    >
                      Pridėti prie mėgstamų
                    </button>
                  </>
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
