import React, { useEffect, useState } from "react";
import Loading from "../../components/loading";
import useAuth from "../../hooks/useAuth";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./Profilis.css";

interface Profile {
  id: number;
  vardas: string;
  pavarde: string;
  elektroninisPastas: string;
  lytis: string;
  klientas?: {
    miestas: string;
    adresas: string;
    pastoKodas: number;
    gimimoData: string;
  };
}

interface Taskai {
  id: number;
  kiekis: number;
}

interface Parduotuve {
  id: number;
  adresas: string;
}

const Profilis: React.FC = () => {
  const { user } = useAuth(); // Current user details
  const [profile, setProfile] = useState<Profile | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [taskai, setTaskai] = useState<number>(0);
  const [addPoints, setAddPoints] = useState<string>("");
  const [validationError, setValidationError] = useState<string | null>(null);
  const [urlId, setUrlId] = useState<number | null>(null); // ID from URL

  // Vadybininkas state
  const [parduotuveList, setParduotuveList] = useState<Parduotuve[]>([]);
  const [skyrius, setSkyrius] = useState<string>("");
  const [selectedParduotuve, setSelectedParduotuve] = useState<string>("");
  const [isVadybininkas, setIsVadybininkas] = useState<boolean>(false);

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
  
    if (!id) {
      setError("Nenurodytas naudotojo ID.");
      setLoading(false);
      return;
    }
  
    setUrlId(Number(id));
  
    const fetchData = async () => {
      try {
        // Fetch Profile
        const profileRes = await fetch(`https://localhost:5210/api/Profilis/${id}`);
        if (!profileRes.ok) throw new Error("Nepavyko gauti naudotojo profilio duomenų.");
        const profileData: Profile = await profileRes.json();
        setProfile(profileData);
  
        // Fetch Taskai
        const taskaiRes = await fetch(`https://localhost:5210/api/Taskai/Naudotojas/${profileData.id}`);
        if (!taskaiRes.ok) throw new Error("Nepavyko gauti naudotojo taškų.");
        const taskaiData: Taskai[] = await taskaiRes.json();
        const totalTaskai = taskaiData.reduce((sum, t) => sum + t.kiekis, 0);
        setTaskai(totalTaskai);
  
        // Fetch Vadybininkas status from DB
        const vadybininkasRes = await fetch(`https://localhost:5210/api/Profilis/is-vadybininkas/${id}`);
        setIsVadybininkas(vadybininkasRes.ok);
  
        // Fetch Parduotuve list
        const parduotuveRes = await fetch(`https://localhost:5210/api/Profilis/all-parduotuves`);
        if (!parduotuveRes.ok) throw new Error("Nepavyko gauti parduotuvių sąrašo.");
        const parduotuves: Parduotuve[] = await parduotuveRes.json();
        setParduotuveList(parduotuves);
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

    if (points > 1000) {
      setValidationError("Vienu metu galite pridėti ne daugiau kaip 1000 taškų.");
      return;
    }

    if (points < 0 && Math.abs(points) > taskai) {
      setValidationError("Negalite atimti daugiau taškų nei klientas turi.");
      return;
    }

    try {
      const response = await fetch(`https://localhost:5210/api/Taskai/AddPoints`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          naudotojasId: profile.id,
          kiekis: points,
        }),
      });

      if (!response.ok) throw new Error("Nepavyko pridėti taškų.");
      const newTaskai: Taskai = await response.json();
      setTaskai(taskai + newTaskai.kiekis);
      setAddPoints("");
      setValidationError(null);
      toast.success("Taškai sėkmingai pridėti!");
    } catch (e: any) {
      setError(e.message);
    }
  };

  const handleDeleteAccount = async () => {
    if (!profile) {
      setError("Profilio duomenys nerasti.");
      return;
    }
  
    if (!window.confirm("Ar tikrai norite ištrinti šį naudotoją? Šis veiksmas yra negrįžtamas.")) {
      return;
    }
  
    try {
      const token = localStorage.getItem("jwt"); // Assuming token is stored in localStorage
  
      const response = await fetch(`https://localhost:5210/api/Profilis/delete/${profile.id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`, // Include the JWT token
        },
      });
  
      if (!response.ok) throw new Error("Nepavyko ištrinti naudotojo paskyros.");
  
      toast.success("Naudotojo paskyra sėkmingai ištrinta.");
      setProfile(null);
    } catch (e: any) {
      setError(e.message);
      toast.error("Įvyko klaida bandant ištrinti paskyrą.");
    }
  };
  


  const handleMakeVadybininkas = async () => {
    if (!selectedParduotuve || !skyrius) {
      toast.error("Prašome užpildyti visus laukus.");
      return;
    }

    try {
      const response = await fetch(`https://localhost:5210/api/Profilis/add-vadybininkas`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          naudotojasId: profile?.id,
          skyrius: skyrius,
          parduotuveId: parseInt(selectedParduotuve),
        }),
      });

      if (!response.ok) throw new Error("Nepavyko nustatyti vadybininko.");
      toast.success("Naudotojas sėkmingai tapo vadybininku!");
      setIsVadybininkas(true);
    } catch (e: any) {
      toast.error(e.message);
    }
  };

  if (loading) return <Loading />;
  if (error) return <div className="alert alert-danger mt-5">{error}</div>;

  return (
    <div className="profilis-container">
      <h1 className="profilis-title">Profilis</h1>
      {profile && (
        <div className="profilis-info">
          {isVadybininkas &&  (
        <p className="vadybininkas-status"><strong>Vadybininkas</strong></p>
          )}
          <h2>
            {profile.vardas} {profile.pavarde}
          </h2>
          <p><strong>El. paštas:</strong> {profile.elektroninisPastas}</p>
          <p><strong>Lytis:</strong> {profile.lytis || "Nenurodyta"}</p>
          {profile.klientas ? (
            <>
              <p><strong>Miestas:</strong> {profile.klientas.miestas}</p>
              <p><strong>Adresas:</strong> {profile.klientas.adresas}</p>
              <p><strong>Pašto kodas:</strong> {profile.klientas.pastoKodas}</p>
              <p><strong>Gimimo data:</strong> {new Date(profile.klientas.gimimoData).toLocaleDateString()}</p>
            </>
          ) : (
            <p><em>Kliento duomenų nėra.</em></p>
          )}
          <p><strong>Turimi taškai:</strong> {taskai}</p>
        </div>
      )}
      
      { user?.id === profile?.id && (
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
      )}
      {user?.administratorius && (
        <>
          <div className="add-points-section">
            <h3>Pridėti taškų</h3>
            <input
              type="number"
              value={addPoints}
              onChange={(e) => setAddPoints(e.target.value)}
              className="form-control mb-3"
              placeholder="Įveskite taškų kiekį (iki 1000)"
            />
            {validationError && <div className="validation-error">{validationError}</div>}
            <button className="add-points-button" onClick={handleAddPoints}>
              Pridėti taškų
            </button>
          </div>
          {!isVadybininkas && parduotuveList.length > 0 &&  user?.id !== profile?.id && (
            <div className="add-vadybininkas-section mt-4">
              <h3>Pridėti vadybininko informaciją</h3>
              <input
                type="text"
                placeholder="Skyrius"
                value={skyrius}
                onChange={(e) => setSkyrius(e.target.value)}
                className="form-control mb-2"
              />
              <select
                value={selectedParduotuve}
                onChange={(e) => setSelectedParduotuve(e.target.value)}
                className="form-control mb-2"
              >
                <option value="">Pasirinkite parduotuvę</option>
                {parduotuveList.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.adresas}
                  </option>
                ))}
              </select>
              <button className="add-points-button" onClick={handleMakeVadybininkas}>
                Patvirtinti vadybininką
              </button>
            </div>
          )}
          {isVadybininkas && (
            <p className="alert alert-success mt-3">Šis naudotojas yra vadybininkas.</p>
          )}
        </>

        
      )}
      {user?.administratorius && profile && (
        <>
          <button className="delete-account-button btn btn-danger" onClick={handleDeleteAccount}>
            Ištrinti paskyrą
          </button>
        </>
      )}

    </div>
  );
};

export default Profilis;
