import React, { useEffect, useState } from "react";
import axios from "axios";

interface ParduotuvesList {
  id: number;
  pavadinimas: string;
}

const PridetiPreke = () => {
  const [title, setTitle] = useState("");
  const [price, setPrice] = useState("");
  const [expireDate, setExpireDate] = useState("");
  const [stores, setStores] = useState<ParduotuvesList[]>([]); // List of stores
  const [selectedStores, setSelectedStores] = useState<
    { parduotuveId: number; kiekis: number }[]
  >([]);

  useEffect(() => {
    // Fetch list of stores from the backend
    const fetchStores = async () => {
      try {
        const response = await axios.get("https://localhost:5210/api/Preke/parduotuvesList");
        setStores(response.data);
        console.log(response.data);
      } catch (error) {
        console.error("Klaida gaunant parduotuves:", error);
      }
    };

    fetchStores();
  }, []);

  const handleAddStore = () => {
    // Add a new blank entry for store
    setSelectedStores([...selectedStores, { parduotuveId: 0, kiekis: 0 }]);
  };

  const handleStoreChange = (index: number, field: string, value: any) => {
    const updatedStores = [...selectedStores];
    updatedStores[index] = {
      ...updatedStores[index],
      [field]: value,
    };
    setSelectedStores(updatedStores);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const newPreke = {
      pavadinimas: title,
      kaina: parseFloat(price),
      galiojimoData: expireDate,
      prekiuParduotuves: selectedStores,
    };

    console.log("Pridedama prekė:", newPreke);

    try {
      await axios.post("/api/prekiu/preke", newPreke);
      alert("Prekė sėkmingai pridėta!");
      window.location.href = "/prekes";
    } catch (error) {
      console.error("Klaida pridedant prekę:", error);
      alert("Nepavyko pridėti prekės.");
    }
  };

  return (
    <div className="container">
      <h1>Pridėti Prekę</h1>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Pavadinimas</label>
          <input
            type="text"
            className="form-control"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite pavadinimą"
          />
        </div>

        <div className="form-group">
          <label>Kaina</label>
          <input
            type="text"
            className="form-control"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            placeholder="Įveskite kainą"
          />
        </div>

        <div className="form-group">
          <label>Galiojimo data</label>
          <input
            type="date"
            className="form-control"
            value={expireDate}
            onChange={(e) => setExpireDate(e.target.value)}
          />
        </div>

        <h3>Parduotuvės</h3>
        {selectedStores.map((store, index) => (
          <div key={index} className="row mb-2">
            <div className="col">
              <label>Parduotuvė</label>
              <select
                className="form-control"
                value={store.parduotuveId}
                onChange={(e) =>
                  handleStoreChange(index, "parduotuveId", Number(e.target.value))
                }
              >
                <option value={0}>Pasirinkite parduotuvę</option>
                {stores.map((store) => (
                  <option key={store.id} value={store.id}>
                    {store.pavadinimas}
                  </option>
                ))}
              </select>
            </div>
            <div className="col">
              <label>Kiekis</label>
              <input
                type="number"
                className="form-control"
                value={store.kiekis}
                onChange={(e) =>
                  handleStoreChange(index, "kiekis", Number(e.target.value))
                }
                placeholder="Įveskite kiekį"
              />
            </div>
          </div>
        ))}

        <button
          type="button"
          onClick={handleAddStore}
          className="btn btn-secondary mb-3"
        >
          Pridėti parduotuvę
        </button>

        <button type="submit" className="btn btn-primary">
          Pridėti
        </button>
      </form>
    </div>
  );
};

export default PridetiPreke;