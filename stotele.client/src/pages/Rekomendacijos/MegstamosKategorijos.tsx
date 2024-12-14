import React, { useState, useEffect } from "react";

type Product = {
  id: number;
  pridejimoData: string;
  kategorijaId: number;
  kategorijaPavadinimas: string;
  kategorijaAprasymas: string;
};

const MegstamosKategorijos = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [refetch, setRefetch] = useState<boolean>(false);

  useEffect(() => {
    const user = localStorage.getItem("user");

    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
    } else {
      console.log("Naudotojas neprisijungęs");
    }
  }, []);

  const fetchData = async () => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/megstamos-kategorijos/${userId}`
      );
      const data = await response.json();
      const sortedProducts = data.sort((a: Product, b: Product) => a.id - b.id);
      setProducts(sortedProducts);
    } catch (error) {
      console.error("Klaida gaunant megstamas kategorijas", error);
    }
  };

  const handleDeleteCategory = async (categoryId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    try {
      const response = await fetch(
        `https://localhost:5210/api/Rekomendacija/megstamos-kategorijos/istrinti/${userId}/${categoryId}`,
        {
          method: "DELETE",
        }
      );

      if (response.ok) {
        setRefetch((prev) => !prev);
        alert("Megstama kategorija istrinta");
      } else {
        alert("Megstamos kategorijos nepavyko istrinti");
      }
    } catch (error) {
      console.error("Klaida istrinant megstama kategorija", error);
    }
  };

  useEffect(() => {
    if (userId !== null) {
      fetchData();
    }
  }, [userId, refetch]);

  return (
    <div>
      <h1>Mėgstamos kategorijos</h1>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Kategorija</th>
            <th>Aprasymas</th>
            <th>Veiksmas</th>
          </tr>
        </thead>
        <tbody>
          {products.map((item, index) => (
            <tr key={item.id}>
              <td>{item.id}</td>
              <td>{item.kategorijaPavadinimas}</td>
              <td>{item.kategorijaAprasymas}</td>
              <td>
                <button onClick={() => handleDeleteCategory(item.kategorijaId)}>
                  Pašalinti
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default MegstamosKategorijos;
