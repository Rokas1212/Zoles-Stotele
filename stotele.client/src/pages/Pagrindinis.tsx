import React, { useState, useEffect } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min";
import defaultImage from "/default_no_photo.jpg";
import {
  FaShoppingCart,
  FaTags,
  FaTruck,
  FaUserEdit,
  FaBox,
  FaThLarge,
  FaInfo,
  FaSearch,
} from "react-icons/fa"; // Add icons from react-icons
import { toast } from "react-toastify";
import { Navigate, useNavigate } from "react-router-dom";
import { addToCart } from "../apiServices/cart";

type Product = {
  id: number;
  kaina: number;
  pavadinimas: string;
  kodas: number;
  galiojimoData: string;
  kiekis: number;
  ismatavimai: string;
  nuotraukosUrl: string;
  garantinisLaikotarpis: string;
  aprasymas: string;
  rekomendacijosSvoris: number;
  mase: number;
  vadybininkasId: number;
  vadybininkas: string | null;
};
const ITEMS_PER_PAGE = 6;
const Pagrindinis = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [refetch, setRefetch] = useState<boolean>(false);
  const [allProducts, setAllProducts] = useState<Product[]>([]);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const navigate = useNavigate();

  useEffect(() => {
    const user = localStorage.getItem("user");

    if (user) {
      const userObject = JSON.parse(user);
      setUserId(userObject.id);
    } else {
      console.log("Naudotojas neprisijungęs");
    }

    fetchAllProducts();
  }, []);

  const fetchAllProducts = async () => {
    try {
      const response = await fetch("/api/Preke/PrekesList");
      const data = await response.json();
      setAllProducts(data || []);
    } catch (error) {
      console.error("Klaida gaunant prekes", error);
    }
  };

  // Pagination Logic
  const indexOfLastProduct = currentPage * ITEMS_PER_PAGE;
  const indexOfFirstProduct = indexOfLastProduct - ITEMS_PER_PAGE;
  const currentProducts = allProducts.slice(
    indexOfFirstProduct,
    indexOfLastProduct
  );

  const totalPages = Math.ceil(allProducts.length / ITEMS_PER_PAGE);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const fetchData = async () => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    try {
      const response = await fetch(
        `/api/Rekomendacija/rekomendacijos/${userId}`
      );
      const data = await response.json();
      setProducts(data.prekes || []);
    } catch (error) {
      console.error("Klaida gaunant rekomenduojamas prekes", error);
    }
  };

  const handleBlockRecommendation = async (productId: number) => {
    if (userId === null) {
      console.log("User ID not found.");
      return;
    }

    const isConfirmed = window.confirm(
      "Ar tikrai norite užblokuoti šią rekomendaciją?"
    );

    if (!isConfirmed) {
      return;
    }

    try {
      const response = await fetch(
        `/api/Rekomendacija/blokuotos-rekomendacijos/uzblokuoti/${userId}/${productId}`,
        {
          method: "PUT",
        }
      );

      if (response.ok) {
        setRefetch((prev) => !prev);
        alert("Rekomendacija sėkmingai užblokuota");
      } else {
        alert("Nepavyko užblokuoti rekomendacijos");
      }
    } catch (error) {
      console.error("Klaida blokuojant rekomendaciją", error);
    }
  };

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

  useEffect(() => {
    if (userId !== null) {
      fetchData();
    }
  }, [userId, refetch]);

  return (
    <div className="container-fluid p-4">
      <h1 className="mb-4 text-center">Žolės Stotelė!</h1>
      <div className="row">
        {/* Sidebar */}
        <div className="col-md-3">
          <div className="gridbox-container mb-5">
            {/* Grid layout for links */}
            <div className="row row-cols-2 row-cols-md-3 g-4">
              <div className="col">
                <div className="d-flex flex-column align-items-center card-link-container">
                  <FaShoppingCart
                    size={30}
                    className="mb-2 icon text-primary"
                  />
                  <a href="/krepselis" className="nav-link text-primary">
                    Krepšelis
                  </a>
                </div>
              </div>
              <div className="col">
                <div className="d-flex flex-column align-items-center card-link-container">
                  <FaTags size={30} className="mb-2 icon text-success" />
                  <a href="/nuolaidos" className="nav-link text-primary">
                    Nuolaidos
                  </a>
                </div>
              </div>
              <div className="col">
                <div className="d-flex flex-column align-items-center card-link-container">
                  <FaTruck size={30} className="mb-2 icon text-info" />
                  <a href="/uzsakymai" className="nav-link text-primary">
                    Užsakymai
                  </a>
                </div>
              </div>
              <div className="col">
                <div className="d-flex flex-column align-items-center card-link-container">
                  <FaBox size={30} className="mb-2 icon text-danger" />
                  <a href="/prekes" className="nav-link text-primary">
                    Prekės
                  </a>
                </div>
              </div>
              <div className="col">
                <div className="d-flex flex-column align-items-center card-link-container">
                  <FaThLarge size={30} className="mb-2 icon text-secondary" />
                  <a
                    href="/prekiu-kategorijos"
                    className="nav-link text-primary"
                  >
                    Visos kategorijos
                  </a>
                </div>
              </div>
            </div>
          </div>

          {/* Recommended Products */}
          <h2 className="mb-4 text-center">Rekomenduojamos prekės</h2>
          <div className="list-group">
            {products.map((product, index) => (
              <div
                key={index}
                className="d-flex align-items-center border rounded p-3 shadow-sm mb-3"
              >
                <img
                  src={product.nuotraukosUrl || defaultImage}
                  alt={product.pavadinimas}
                  className="img-fluid rounded me-3"
                  style={{
                    width: "100px",
                    height: "100px",
                    objectFit: "cover",
                  }}
                  onError={(e) => {
                    (e.target as HTMLImageElement).src = defaultImage;
                  }}
                />
                <div>
                  <p className="fw-bold mb-2">{product.pavadinimas}</p>
                  <button
                    onClick={() => handleBlockRecommendation(product.id)}
                    className="btn btn-outline-danger btn-sm mt-2"
                  >
                    Blokuoti rekomendaciją
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Main Content */}
        <div className="col-md-9">
          {/* Pagination */}
          <nav aria-label="Page navigation" className="mt-4">
            <ul className="pagination justify-content-center">
              {Array.from({ length: totalPages }, (_, index) => (
                <li
                  key={index}
                  className={`page-item ${
                    currentPage === index + 1 ? "active" : ""
                  }`}
                >
                  <button
                    className="page-link"
                    onClick={() => handlePageChange(index + 1)}
                  >
                    {index + 1}
                  </button>
                </li>
              ))}
            </ul>
          </nav>
          <div className="row row-cols-2 g-4">
            {currentProducts.map((product, index) => (
              <div key={index} className="col">
                <div className="card h-100 shadow-sm">
                  <img
                    src={product.nuotraukosUrl || defaultImage}
                    alt={product.pavadinimas}
                    className="card-img-top"
                    style={{
                      width: "100%",
                      height: "180px", // Fixed height for all images
                      objectFit: "cover", // Maintain aspect ratio, fill the box
                    }}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = defaultImage;
                    }}
                  />
                  <div className="card-body d-flex flex-column justify-content-between">
                    <h5 className="fw-bold mb-2 text-primary">
                      {product.pavadinimas}
                    </h5>
                    <p className="text-muted mb-1">
                      <strong>Kaina:</strong> € {product.kaina.toFixed(2)}
                    </p>
                    <p className="text-muted mb-1">
                      <strong>Likutis:</strong> {product.kiekis}
                    </p>
                    <p className="text-muted mb-2">
                      <strong>Aprašymas:</strong> {product.aprasymas}
                    </p>
                    <div className="mt-2 d-flex justify-content-between">
                      <button
                        onClick={() => handleAddToCart(product.id.toString())}
                        className="btn btn-primary btn-sm"
                      >
                        <FaShoppingCart /> Į krepšelį
                      </button>
                      <button
                        onClick={() => navigate(`/preke?id=${product.id}`)}
                        className="btn btn-outline-primary btn-sm"
                      >
                        <FaSearch /> Daugiau
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Pagrindinis;
