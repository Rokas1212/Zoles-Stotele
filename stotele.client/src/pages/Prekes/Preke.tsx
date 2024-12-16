import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate, useSearchParams } from "react-router-dom";
import "./preke.css";
import Loading from "../../components/loading";
import useAuth from "../../hooks/useAuth";
import MapComponent from "../../components/mapComponent";
import { FaShoppingCart } from "react-icons/fa";
import { addToCart } from "../../apiServices/cart";
import { toast } from "react-toastify";
import ConfirmationModal from "../../components/confirmationModal";

interface Preke {
  id: number;
  pavadinimas: string;
  kaina: number;
  aprasymas: string;
  nuotraukosUrl: string;
  kodas: number;
  ismatavimai: string;
  mase: number;
  garantinisLaikotarpis: Date;
  galiojimoData: Date;
}

interface Kategorija {
  pavadinimas: string;
  id: number;
}

const Preke: React.FC = () => {
  const { user } = useAuth(); // Access the authenticated user
  const [product, setProduct] = useState<Preke | null>(null);
  const [addresses, setAddresses] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [categories, setCategories] = useState<Kategorija[]>([]);
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");

  useEffect(() => {
    console.log("Categories state:", categories);
  }, [categories]);

  useEffect(() => {
    const fetchPreke = async () => {
      if (!id) {
        setError("Nenurodytas prekės ID.");
        setLoading(false);
        return;
      }

      try {
        const response = await axios.get(
          `https://localhost:5210/api/Preke/${id}`
        );
        console.log(response.data);
        setProduct(response.data.preke);
        setCategories(response.data.kategorijos);
      } catch (err: any) {
        setError(
          err.response?.data ||
            "Nepavyko gauti prekės informacijos iš serverio."
        );
      }
    };

    const fetchAddresses = async () => {
      if (!id) return;

      try {
        const response = await axios.get<string[]>(
          `https://localhost:5210/api/Preke/${id}/addresses`
        );
        setAddresses(response.data);
      } catch (err: any) {
        console.error(
          "Klaida gaunant parduotuvių adresus:",
          err.response?.data
        );
      }
    };

    setLoading(true);
    Promise.all([fetchPreke(), fetchAddresses()]).finally(() => {
      setLoading(false);
    });
  }, [id]);

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  if (!product) {
    return <div className="error">Prekė nerasta.</div>;
  }

  const handleAddToCart = async (id: string) => {
    try {
      await addToCart(id);
      toast.success(
        <div className="d-flex align-items-center justify-content-between">
          <span className="mb-0">Prekė pridėta</span>
          <button
            className="btn btn-primary btn-sm"
            onClick={() => {
              navigate("/krepselis");
              toast.dismiss();
            }}
          >
            Eiti į krepšelį
          </button>
        </div>,
        {
          autoClose: 5000,
          hideProgressBar: false,
          closeOnClick: false,
          pauseOnHover: true,
          draggable: true,
        }
      );
    } catch (error) {
      console.error("Klaida: ", error);
      toast.error("Nepavyko pridėti prekės į krepšelį.", {
        position: "top-right",
        autoClose: 3000,
      });
    }
  };

  const handleDeleteProduct = async () => {
    try {
      const getConfirmation = window.confirm(
        "Ar tikrai norite ištrinti prekę?"
      );
      if (!getConfirmation) return;
      const response = await axios.delete(`https://localhost:5210/api/Preke/`, {
        params: { id },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      if (response.status === 204) {
        toast.success("Prekė ištrinta.");
        navigate("/prekes");
      }
    } catch (error: any) {
      console.error("Klaida trinant prekę:", error.response?.data);
      toast.error("Nepavyko ištrinti prekės.");
    }
  };

  return (
    <div className="container py-4">
      {/* Product Title */}
      <div className="text-center mb-4">
        <h1 className="display-4">{product.pavadinimas}</h1>
      </div>

      {/* Product Image and Price */}
      <div className="row mb-4">
        <div className="col-md-6">
          <img
            src={product.nuotraukosUrl}
            alt={product.pavadinimas}
            className="img-fluid rounded shadow-sm"
          />
        </div>
        <div className="col-md-6 d-flex flex-column justify-content-center">
          <h3 className="text-success">
            <strong>Kaina:</strong> €{product.kaina.toFixed(2)}
          </h3>
          <p className="lead">{product.aprasymas}</p>

          {/* Add to Cart Button */}
          <div className="d-flex align-items-center justify-content-start mt-3">
            <button
              onClick={() => handleAddToCart(product.id.toString())}
              className="btn btn-primary btn-lg"
            >
              <FaShoppingCart />
            </button>
          </div>
        </div>
      </div>

      {/* Technical Specifications */}
      <h2 className="mb-3">Techninės Specifikacijos</h2>
      <div className="card mb-4 shadow-sm">
        <div className="card-body">
          <ul className="list-group list-group-flush">
            <li className="list-group-item">
              <strong>Prekės Kodas:</strong> {product.kodas}
            </li>
            <li className="list-group-item">
              <strong>Išmatavimai:</strong> {product.ismatavimai}
            </li>
            <li className="list-group-item">
              <strong>Masė:</strong> {product.mase} kg
            </li>
            <li className="list-group-item">
              <strong>Garantinis laikotarpis, nepatiks - grąžink!:</strong>{" "}
              {new Date(product.garantinisLaikotarpis).toLocaleDateString()}
            </li>
            <li className="list-group-item">
              <strong>Galiojimo data:</strong>{" "}
              {new Date(product.galiojimoData).toLocaleDateString()}
            </li>
          </ul>
        </div>
      </div>

      {/* Categories */}
      {categories.length > 0 && (
        <>
          <h2 className="mb-3">Kategorijos</h2>
          <div className="card mb-4 shadow-sm">
            <div className="card-header bg-primary text-white">
              Prekės Kategorijos
            </div>
            <ul className="list-group list-group-flush">
              {categories.map((category) => (
                <li className="list-group-item" key={category.id}>
                  {category.pavadinimas}
                </li>
              ))}
            </ul>
          </div>
        </>
      )}

      {/* Admin Button */}
      {user?.administratorius && (
        <div className="text-center mb-4">
          <a href={`/redaguoti-preke?id=${product.id}`}>
            <button className="btn btn-warning btn-lg">Redaguoti</button>
          </a>
          <button
            onClick={handleDeleteProduct}
            className="btn btn-warning btn-lg"
          >
            Ištrinti prekę
          </button>
        </div>
      )}

      {/* Map Component */}
      <div>
        <h2 className="mb-3">Prekę turime šiose lokacijose</h2>
        {addresses.length > 0 ? (
          <div className="map-container border rounded shadow-sm">
            <MapComponent addresses={addresses} />
          </div>
        ) : (
          <p className="text-danger">
            Šios prekės nėra parduotuvių sandėliuose.
          </p>
        )}
      </div>
    </div>
  );
};

export default Preke;
