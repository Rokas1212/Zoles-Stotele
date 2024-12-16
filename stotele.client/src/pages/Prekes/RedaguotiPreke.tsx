import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import "./RedaguotiPreke.css";

interface Kategorija {
  id: number;
  pavadinimas: string;
}

const RedaguotiPreke = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [product, setProduct] = useState<any>(null);

  const [pavadinimas, setPavadinimas] = useState("");
  const [kaina, setKaina] = useState(0);
  const [galiojimoData, setGaliojimoData] = useState("");
  const [aprasymas, setAprasymas] = useState("");
  const [nuotraukosUrl, setNuotraukosUrl] = useState("");
  const [garantinisLaikotarpis, setGarantinisLaikotarpis] = useState("");

  const [prekiuKategorijos, setPrekiuKategorijos] = useState<Kategorija[]>([]);
  const [availableCategories, setAvailableCategories] = useState<Kategorija[]>(
    []
  );

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        setLoading(true);

        const response = await fetch(`https://localhost:5210/api/Preke/${id}`);
        const data = await response.json();

        if (response.ok) {
          setProduct(data.preke);
          setPavadinimas(data.preke.pavadinimas);
          setKaina(data.preke.kaina);
          setAprasymas(data.preke.aprasymas);
          setNuotraukosUrl(data.preke.nuotraukosUrl);
          setGaliojimoData(
            new Date(data.preke.galiojimoData).toISOString().split("T")[0]
          );
          setGarantinisLaikotarpis(
            new Date(data.preke.garantinisLaikotarpis)
              .toISOString()
              .split("T")[0]
          );
          setPrekiuKategorijos(data.kategorijos || []);
        } else {
          setError("Prekė nerasta");
        }
      } catch (err) {
        setError("Error loading the product");
      } finally {
        setLoading(false);
      }
    };

    const fetchCategories = async () => {
      try {
        const response = await fetch(
          "https://localhost:5210/api/Kategorija/kategorijos"
        );
        const data = await response.json();
        setAvailableCategories(data);
      } catch (err) {
        console.error("Klaida gaunant kategorijas:", err);
      }
    };

    if (id) {
      fetchProduct();
      fetchCategories();
    }
  }, [id]);

  const handleAddCategory = (categoryId: number) => {
    if (prekiuKategorijos.some((cat) => cat.id === categoryId)) {
      alert("Ši kategorija jau pridėta.");
      return;
    }

    const category = availableCategories.find((cat) => cat.id === categoryId);
    if (category) {
      setPrekiuKategorijos([...prekiuKategorijos, category]);
    }
  };

  const handleDeleteCategory = (index: number) => {
    const updatedCategories = [...prekiuKategorijos];
    updatedCategories.splice(index, 1);
    setPrekiuKategorijos(updatedCategories);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const updatedProduct = {
      Pavadinimas: pavadinimas,
      Kaina: kaina,
      GaliojimoData: galiojimoData,
      Aprasymas: aprasymas,
      NuotraukosUrl: nuotraukosUrl,
      GarantinisLaikotarpis: garantinisLaikotarpis,
      PrekiuKategorijos: prekiuKategorijos.map((cat) => ({
        kategorijaId: cat.id,
      })),
    };
    try {
      const response = await fetch(`https://localhost:5210/api/Preke/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(updatedProduct),
      });

      if (response.ok) {
        navigate(`/preke?id=${id}`);
      } else {
        const errorData = await response.json();
        setError(errorData.message || "Failed to update product");
      }
    } catch (err) {
      setError("Error updating the product");
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  if (!product) {
    return <div>Prekė nerasta</div>;
  }

  return (
    <div className="form-container">
      <h1>Redaguoti Prekę</h1>
      <form onSubmit={handleSubmit} className="product-form">
        {/* Product Fields */}
        <div className="form-group">
          <label>Pavadinimas</label>
          <input
            type="text"
            value={pavadinimas}
            onChange={(e) => setPavadinimas(e.target.value)}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label>Kaina</label>
          <input
            type="number"
            value={kaina}
            onChange={(e) => setKaina(parseFloat(e.target.value))}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label>Galiojimo Data</label>
          <input
            type="date"
            value={galiojimoData}
            onChange={(e) => setGaliojimoData(e.target.value)}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label>Aprašymas</label>
          <textarea
            value={aprasymas}
            onChange={(e) => setAprasymas(e.target.value)}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label>Nuotraukos URL</label>
          <input
            type="text"
            value={nuotraukosUrl}
            onChange={(e) => setNuotraukosUrl(e.target.value)}
            className="form-input"
          />
        </div>
        <div className="form-group">
          <label>Garantinis Laikotarpis</label>
          <input
            type="date"
            value={garantinisLaikotarpis}
            onChange={(e) => setGarantinisLaikotarpis(e.target.value)}
            className="form-input"
          />
        </div>

        {/* Categories */}
        <div className="form-group categories-section">
          <h3>Prekių Kategorijos</h3>
          {prekiuKategorijos.map((category, index) => (
            <div key={index} className="category-item">
              <span>{category.pavadinimas}</span>
              <button
                type="button"
                onClick={() => handleDeleteCategory(index)}
                className="delete-category-btn"
              >
                Pašalinti
              </button>
            </div>
          ))}

          <div className="add-category-section">
            <select
              onChange={(e) => handleAddCategory(parseInt(e.target.value))}
              defaultValue=""
              className="category-select"
            >
              <option value="" disabled>
                Pasirinkite kategoriją
              </option>
              {availableCategories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.pavadinimas}
                </option>
              ))}
            </select>
          </div>
        </div>

        <button type="submit" className="submit-btn">
          Išsaugoti
        </button>
      </form>
    </div>
  );
};

export default RedaguotiPreke;
