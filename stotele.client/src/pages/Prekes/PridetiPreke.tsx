import React, { useEffect, useState } from "react";
import axios from "axios";

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
        `https://localhost:5210/api/Kategorija/kategorijos`
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
    <div className="container mt-5">
      <h1 className="text-center mb-4">Pridėti Prekę</h1>
      <form onSubmit={handleSubmit}>
        {/* Title Input */}
        <div className="mb-3">
          <label htmlFor="title" className="form-label">
            Pavadinimas
          </label>
          <input
            type="text"
            className="form-control"
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Įveskite pavadinimą"
            required
          />
        </div>

        {/* Price Input */}
        <div className="mb-3">
          <label htmlFor="price" className="form-label">
            Kaina
          </label>
          <input
            type="number"
            className="form-control"
            id="price"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            placeholder="Įveskite kainą"
            required
          />
        </div>

        {/* Expiry Date */}
        <div className="mb-3">
          <label htmlFor="expireDate" className="form-label">
            Galiojimo data
          </label>
          <input
            type="date"
            className="form-control"
            id="expireDate"
            value={expireDate}
            onChange={(e) => setExpireDate(e.target.value)}
            required
          />
        </div>

        {/* Description */}
        <div className="mb-3">
          <label htmlFor="description" className="form-label">
            Aprašymas
          </label>
          <textarea
            className="form-control"
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Įveskite aprašymą"
            rows={4}
            required
          />
        </div>

        {/* Measurements */}
        <div className="mb-3">
          <label htmlFor="measurements" className="form-label">
            Išmatavimai
          </label>
          <input
            type="text"
            className="form-control"
            id="measurements"
            value={measurements}
            onChange={(e) => setMeasurements(e.target.value)}
            placeholder="Įveskite išmatavimus"
          />
        </div>

        {/* Product Code */}
        <div className="mb-3">
          <label htmlFor="code" className="form-label">
            Prekės Kodas
          </label>
          <input
            type="text"
            className="form-control"
            id="code"
            value={code}
            onChange={(e) => setCode(e.target.value)}
            placeholder="Įveskite kodą"
            required
          />
        </div>

        {/* Warranty */}
        <div className="mb-3">
          <label htmlFor="warrantyUntil" className="form-label">
            Garantinis laikotarpis
          </label>
          <input
            type="date"
            className="form-control"
            id="warrantyUntil"
            value={warrantyUntil}
            onChange={(e) => setWarrantyUntil(e.target.value)}
          />
        </div>

        {/* Recommendation Weight */}
        <div className="mb-3">
          <label htmlFor="recommendationWeight" className="form-label">
            Rekomendacijos svoris
          </label>
          <input
            type="number"
            className="form-control"
            id="recommendationWeight"
            value={recommendationWeight}
            onChange={(e) => setRecommendationWeight(e.target.value)}
            placeholder="Įveskite svorį"
            required
          />
        </div>

        {/* Mass */}
        <div className="mb-3">
          <label htmlFor="mass" className="form-label">
            Prekės masė
          </label>
          <input
            type="number"
            className="form-control"
            id="mass"
            value={mass}
            onChange={(e) => setMass(e.target.value)}
            placeholder="Įveskite masę"
            required
          />
        </div>

        {/* Image URL */}
        <div className="mb-3">
          <label htmlFor="imageUrl" className="form-label">
            Nuotraukos URL
          </label>
          <input
            type="url"
            className="form-control"
            id="imageUrl"
            value={imageUrl}
            onChange={(e) => setImageUrl(e.target.value)}
            placeholder="Įveskite nuotraukos URL"
          />
        </div>

        {/* Categories */}
        <h3>Kategorijos</h3>
        {selectedCategories.map((category, index) => (
          <div key={index} className="mb-3">
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
        ))}
        <button
          type="button"
          onClick={handleAddCategory}
          className="btn btn-outline-secondary mb-3"
        >
          Pridėti kategoriją
        </button>

        {/* Stores */}
        <h3>Parduotuvės</h3>
        {selectedStores.map((store, index) => (
          <div key={index} className="row mb-3">
            <div className="col-md-6">
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
            <div className="col-md-6">
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
          className="btn btn-outline-secondary mb-3"
        >
          Pridėti parduotuvę
        </button>

        {/* Submit Button */}
        <button type="submit" className="btn btn-primary btn-block mt-4">
          Pridėti
        </button>
      </form>
    </div>
  );
};

export default PridetiPreke;
