import React, { useEffect, useState } from "react";
import Loading from "../../components/loading";
import "./profiliai.css";

interface Profile {
  id: number;
  vardas: string;
  pavarde: string;
  elektroninisPastas: string;
}

const Profiliai: React.FC = () => {
  const [profiles, setProfiles] = useState<Profile[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const response = await fetch("/api/Profilis/klientai");
        if (!response.ok) {
          throw new Error("Klaida gaunant klientų profilius");
        }
        const data = await response.json();

        const sortedData = data.sort((a: Profile, b: Profile) => a.id - b.id);

        setProfiles(sortedData);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchProfiles();
  }, []);

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="error-message">Klaida: {error}</div>;
  }

  return (
    <div className="profiliai-container">
      <h1 className="title">Naudotojų profiliai</h1>
      <div className="table-container">
        <table className="styled-table">
          <thead>
            <tr>
              <th>Naudotojo ID</th>
              <th>Vardas</th>
              <th>Pavardė</th>
              <th>El. paštas</th>
              <th>Veiksmai</th>
            </tr>
          </thead>
          <tbody>
            {profiles.map((profile) => (
              <tr key={profile.id}>
                <td>{profile.id}</td>
                <td>{profile.vardas}</td>
                <td>{profile.pavarde}</td>
                <td>{profile.elektroninisPastas}</td>
                <td>
                  <a
                    href={`/profilis?id=${profile.id}`}
                    className="view-button"
                  >
                    Peržiūrėti
                  </a>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Profiliai;
