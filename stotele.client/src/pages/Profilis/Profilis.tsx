import React, { useEffect, useState } from "react";
import Loading from "../../components/loading";
import useAuth from "../../hooks/useAuth"; // Import useAuth
import "./Profilis.css";

interface Profile {
  id: number;
  vardas: string;
  pavarde: string;
  elektroninisPastas: string;
}

interface Taskai {
  id: number;
  kiekis: number;
}

const Profilis: React.FC = () => {
  const { user } = useAuth(); // Get logged-in user info
  const [profile, setProfile] = useState<Profile | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [taskai, setTaskai] = useState<number>(0);
  const [addPoints, setAddPoints] = useState<string>("");
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");

    if (!id) {
      setError("Nenurodytas naudotojo ID.");
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const profileRes = await fetch(`https://localhost:5210/api/Profilis/${id}`);
        if (!profileRes.ok) {
          throw new Error("Nepavyko gauti naudotojo profilio duomenų.");
        }

        const profileData: Profile = await profileRes.json();
        setProfile(profileData);

        const taskaiRes = await fetch(`https://localhost:5210/api/Taskai/Naudotojas/${profileData.id}`);
        if (!taskaiRes.ok) {
          throw new Error("Nepavyko gauti naudotojo taškų.");
        }

        const taskaiData: Taskai[] = await taskaiRes.json();
        const totalTaskai = taskaiData.reduce((sum, t) => sum + t.kiekis, 0);
        setTaskai(totalTaskai);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleAddPoints = async () => {
    if (!profile) {
      setError("Profilio duomenys nerasti.");
      return;
    }

    const points = parseInt(addPoints, 10);
    if (isNaN(points)) {
      setValidationError("Įveskite teisingą taškų skaičių.");
      return;
    }

    if (points < 0 && Math.abs(points) > taskai) {
      setValidationError("Negalite atimti daugiau taškų nei klientas turi.");
      return;
    }

    try {
      const response = await fetch(`https://localhost:5210/api/Taskai/AddPoints`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          naudotojasId: profile.id,
          kiekis: points,
        }),
      });

      if (!response.ok) {
        throw new Error("Nepavyko pridėti taškų.");
      }

      const newTaskai: Taskai = await response.json();
      setTaskai(taskai + newTaskai.kiekis);
      setAddPoints("");
      setValidationError(null);
    } catch (e: any) {
      setError(e.message);
    }
  };

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="alert alert-danger mt-5">{error}</div>;
  }

  return (
    <div className="profilis-container">
      <h1 className="profilis-title">Profilis</h1>
      {profile && (
        <div className="profilis-info">
          <h2>
            {profile.vardas} {profile.pavarde}
          </h2>
          <p>
            <strong>Turimi taškai:</strong> {taskai}
          </p>
        </div>
      )}
      <ul className="profilis-menu">
        <li>
          <a href="/uzsakymai">Užsakymai</a>
        </li>
        <li>
          <a href="/blokuotos-rekomendacijos">Blokuotos Rekomendacijos</a>
        </li>
        <li>
          <a href="/megstamos-kategorijos">Mėgstamos kategorijos</a>
        </li>
        <li>
          <a href="/redaguoti-profili">Redaguoti profilį</a>
        </li>
      </ul>
      {/* Reveal this section only if the logged-in user is an administrator */}
      {user?.administratorius && (
        <div className="add-points-section">
          <h3>Pridėti taškų</h3>
          <input
            type="text"
            value={addPoints}
            onChange={(e) => setAddPoints(e.target.value)}
            className="form-control mb-3"
            placeholder="Įveskite taškų kiekį"
          />
          {validationError && <div className="validation-error">{validationError}</div>}
          <button className="add-points-button" onClick={handleAddPoints}>
            Pridėti taškų
          </button>
        </div>
      )}
    </div>
  );
};

export default Profilis;
