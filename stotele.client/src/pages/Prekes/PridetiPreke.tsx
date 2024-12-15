import React, { useEffect, useState } from "react";
import axios from "axios";
import PrekiuKategorijos from "./PrekiuKategorijos";

interface ParduotuvesList {
  id: number;
  adresas: string;
}

interface KategorijaList {
  id: number;
  pavadinimas: string;
}

const PridetiPreke = () => {
  const [title, setTitle] = useState("");
  const [price, setPrice] = useState("");
  const [expireDate, setExpireDate] = useState("");
  const [description, setDescription] = useState("");
  const [measurements, setMeasurements] = useState("");
  const [imageUrl, setImageUrl] = useState("");
  const [stores, setStores] = useState<ParduotuvesList[]>([]);
  const [categories, setCategories] = useState<KategorijaList[]>([]);
  const [code, setCode] = useState("");
  const [warrantyUntil, setWarrantyUntil] = useState("");
  const [recommendationWeight, setRecommendationWeight] = useState("");
  const [mass, setMass] = useState("");
  const [selectedStores, setSelectedStores] = useState<
    { parduotuveId: number; kiekis: number }[]
  >([]);
  const [selectedCategories, setSelectedCategories] = useState<
    { kategorijaId: number }[]
  >([]);
  const user = JSON.parse(localStorage.getItem("user") || "{}");
  const userId = user.id;

  useEffect(() => {
    // Fetch list of stores from the backend
    console.log(userId);
    const fetchStores = async () => {
      try {
        const response = await axios.get(
          "https://localhost:5210/api/Preke/parduotuvesList",
          {
            withCredentials: true,
            headers: {
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
          }
        );
        setStores(response.data);
      } catch (error) {
        console.error("Klaida gaunant parduotuves:", error);
      }
    };

    fetchCategories();
    fetchStores();
  }, []);

  const fetchCategories = async () => {
    try {
      const response = await fetch(
        "https://localhost:5210/api/Kategorija/kategorijos"
      );
      const data = await response.json();
      const categories = data.map(
        (category: { id: number; pavadinimas: string }) => ({
          id: category.id,
          pavadinimas: category.pavadinimas,
        })
      );
      setCategories(categories);
    } catch (error) {
      console.error("Klaida gaunant kategorijas", error);
    }
  };

  const handleAddCategory = () => {
    setSelectedCategories([...selectedCategories, { kategorijaId: 0 }]);
  };

  const handleCategoryChange = (index: number, value: any) => {
    const updatedCategories = [...selectedCategories];
    updatedCategories[index] = {
      ...updatedCategories[index],
      kategorijaId: value,
    };
    setSelectedCategories(updatedCategories);
  };

  const handleAddStore = () => {
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
      aprasymas: description,
      ismatavimai: measurements,
      nuotraukosUrl: imageUrl,
      prekiuParduotuves: selectedStores,
      vadybininkasId: userId,
      kodas: code,
      garantinisLaikotarpis: warrantyUntil,
      rekomendacijosSvoris: parseFloat(recommendationWeight),
      mase: parseFloat(mass),
      prekiuKategorijos: selectedCategories,
    };

    console.log("Pridedama prekė:", newPreke);

    try {
      await axios.post("https://localhost:5210/api/Preke", newPreke, {
        withCredentials: true,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
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

        <div className="form-group">
          <label>Aprašymas</label>
          <textarea
            className="form-control"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Įveskite aprašymą"
          />
        </div>

        <div className="form-group">
          <label>Išmatavimai</label>
          <input
            type="text"
            className="form-control"
            value={measurements}
            onChange={(e) => setMeasurements(e.target.value)}
            placeholder="Įveskite išmatavimus"
          />
        </div>

        <div className="form-group">
          <label>Prekės Kodas</label>
          <input
            type="text"
            className="form-control"
            value={code}
            onChange={(e) => setCode(e.target.value)}
            placeholder="Įveskite kodą"
          />
        </div>

        <div className="form-group">
          <label>Garantinis laikotarpis</label>
          <input
            type="date"
            className="form-control"
            value={warrantyUntil}
            onChange={(e) => setWarrantyUntil(e.target.value)}
          />
        </div>

        <div className="form-group">
          <label>Rekomendacijos svoris</label>
          <input
            type="text"
            className="form-control"
            value={recommendationWeight}
            onChange={(e) => setRecommendationWeight(e.target.value)}
            placeholder="Įveskite svorį"
          />
        </div>

        <div className="form-group">
          <label>Prekės masė</label>
          <input
            type="text"
            className="form-control"
            value={mass}
            onChange={(e) => setMass(e.target.value)}
            placeholder="Įveskite masę"
          />
        </div>

        <div className="form-group">
          <label>Nuotraukos URL</label>
          <input
            type="text"
            className="form-control"
            value={imageUrl}
            onChange={(e) => setImageUrl(e.target.value)}
            placeholder="Įveskite nuotraukos URL"
          />
        </div>

        <h3>Kategorijos</h3>
        {selectedCategories.map((category, index) => (
          <div key={index} className="row mb-2">
            <div className="col">
              <label>Kategorija</label>
              <select
                className="form-control"
                value={category.kategorijaId}
                onChange={(e) =>
                  handleCategoryChange(index, Number(e.target.value))
                }
              >
                <option value={0}>Pasirinkite kategoriją</option>
                {categories.map((category) => (
                  <option key={category.id} value={category.id}>
                    {category.pavadinimas}
                  </option>
                ))}
              </select>
            </div>
          </div>
        ))}

        <button
          type="button"
          onClick={handleAddCategory}
          className="btn btn-secondary mb-3"
        >
          Pridėti kategoriją
        </button>

        <h3>Parduotuvės</h3>
        {selectedStores.map((store, index) => (
          <div key={index} className="row mb-2">
            <div className="col">
              <label>Parduotuvė</label>
              <select
                className="form-control"
                value={store.parduotuveId}
                onChange={(e) =>
                  handleStoreChange(
                    index,
                    "parduotuveId",
                    Number(e.target.value)
                  )
                }
              >
                <option value={0}>Pasirinkite parduotuvę</option>
                {stores.map((store) => (
                  <option key={store.id} value={store.id}>
                    {store.adresas}
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
